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

    public CreateRevisionCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateRevisionCommand request, CancellationToken cancellationToken)
    {
        var app = await _context.Apps
            .Where(a => a.Id == request.AppId)
            .SingleOrDefaultAsync(cancellationToken);
        _ = app ?? throw new NotFoundException(nameof(App), request.AppId);

        var entity = new Revision
        {
            AppId = request.AppId,
            App = app,
            RevisionNumber = request.RevisionNumber
        };

        entity.AddDomainEvent(new CreatedEvent<Revision>(entity));

        _context.Revisions.Add(entity);

        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
