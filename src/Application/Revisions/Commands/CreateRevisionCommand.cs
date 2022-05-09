using System.ComponentModel.DataAnnotations;
using Hippo.Application.Common.Exceptions;
using Hippo.Application.Common.Interfaces;
using Hippo.Core.Entities;
using Hippo.Core.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hippo.Application.Revisions.Commands;

public class CreateRevisionCommand : IRequest<Guid>
{
    [Required]
    public Guid AppId { get; set; }

    [Required]
    public string RevisionNumber { get; set; } = "";
}

public class CreateRevisionCommandHandler : IRequestHandler<CreateRevisionCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    private readonly IBindleService _bindleService;

    public CreateRevisionCommandHandler(IApplicationDbContext context, IBindleService bindleService)
    {
        _context = context;
        _bindleService = bindleService;
    }

    public async Task<Guid> Handle(CreateRevisionCommand request, CancellationToken cancellationToken)
    {
        var app = await _context.Apps
            .Where(a => a.Id == request.AppId)
            .SingleOrDefaultAsync(cancellationToken);
        _ = app ?? throw new NotFoundException(nameof(App), request.AppId);

        var revisionDetails = await _bindleService.GetRevisionDetails($"{app.StorageId}/{request.RevisionNumber}");

        var newRevision = new Revision
        {
            AppId = request.AppId,
            App = app,
            RevisionNumber = request.RevisionNumber,
            Description = revisionDetails?.Description,
            Type = revisionDetails?.SpinToml?.Trigger?.Type,
            Base = revisionDetails?.SpinToml?.Trigger?.Base,
        };

        var newRevisionComponents = GetRevisionComponents(newRevision, revisionDetails?.SpinToml?.Component);

        _context.RevisionComponents.AddRange(newRevisionComponents);
        _context.Revisions.Add(newRevision);

        await _context.SaveChangesAsync(cancellationToken);

        return newRevision.Id;
    }

    private static IEnumerable<RevisionComponent> GetRevisionComponents(Revision newRevision, List<Models.RevisionComponent>? components)
    {
        if (components is null || !components.Any())
        {
            return new List<RevisionComponent>();
        }

        return components.Select(c => new RevisionComponent
        {
            Source = c.Source,
            Name = c.Id,
            Route = c.Trigger?.Route ?? "/",
            Revision = newRevision,
        }).ToList();
    }
}
