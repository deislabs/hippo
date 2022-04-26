using System.ComponentModel.DataAnnotations;
using Hippo.Application.Common.Exceptions;
using Hippo.Application.Common.Interfaces;
using Hippo.Core.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hippo.Application.Revisions.Commands;

public class ImportRevisionsCommand : IRequest<List<Revision>>
{
    [Required]
    public Guid AppId { get; set; }
}

public class ImportRevisionsCommandHandler : IRequestHandler<ImportRevisionsCommand, List<Revision>>
{
    private readonly IApplicationDbContext _context;
    private readonly IBindleService _bindleService;

    public ImportRevisionsCommandHandler(IApplicationDbContext context, IBindleService bindleService)
    {
        _context = context;
        _bindleService = bindleService;
    }

    public async Task<List<Revision>> Handle(ImportRevisionsCommand request, CancellationToken cancellationToken)
    {
        var app = await _context.Apps
            .Where(a => a.Id == request.AppId)
            .SingleOrDefaultAsync(cancellationToken);
        _ = app ?? throw new NotFoundException(nameof(App), request.AppId);

        var newRevisions = new List<Revision>();
        if (app.StorageId is null)
        {
            return newRevisions;
        }

        var appRevisions = await _bindleService.GetBindleRevisionNumbers(app.StorageId);
        foreach (var revisionNumber in appRevisions)
        {
            var existingRevision = _context.Revisions
                .FirstOrDefault(r => r.AppId == app.Id && r.RevisionNumber == revisionNumber);
            if (existingRevision is not null)
            {
                continue;
            }

            var entity = new Revision
            {
                AppId = request.AppId,
                RevisionNumber = revisionNumber,
            };

            _context.Revisions.Add(entity);
            newRevisions.Add(entity);
        }

        await _context.SaveChangesAsync(cancellationToken);

        return newRevisions;
    }
}
