using System.ComponentModel.DataAnnotations;
using Hippo.Application.Common.Exceptions;
using Hippo.Application.Common.Interfaces;
using Hippo.Core.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hippo.Application.Revisions.Commands;

public class ImportRevisionsCommand : IRequest
{
    [Required]
    public Guid AppId { get; set; }
}

public class ImportRevisionsCommandHandler : IRequestHandler<ImportRevisionsCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IBindleService _bindleService;

    public ImportRevisionsCommandHandler(IApplicationDbContext context, IBindleService bindleService)
    {
        _context = context;
        _bindleService = bindleService;
    }

    public async Task<Unit> Handle(ImportRevisionsCommand request, CancellationToken cancellationToken)
    {
        var app = await _context.Apps
            .Where(a => a.Id == request.AppId)
            .SingleOrDefaultAsync(cancellationToken);
        _ = app ?? throw new NotFoundException(nameof(App), request.AppId);
        if (app.StorageId is null)
        {
            return Unit.Value;
        }

        var allAppRevisions = await _bindleService.GetBindleRevisionNumbers(app.StorageId);
        var existingRevisions = _context.Revisions.Where(r => r.AppId == app.Id).ToList();
        var missingRevisions = GetMissingRevisions(allAppRevisions, existingRevisions, app.Id);

        _context.Revisions.AddRange(missingRevisions);

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }

    private static IEnumerable<Revision> GetMissingRevisions(IEnumerable<string> allAppRevisions, List<Revision> existingRevisions, Guid appId)
    {
        return allAppRevisions.Where(revision => !existingRevisions.Any(er => er.RevisionNumber == revision))
            .Select(r => new Revision
            {
                AppId = appId,
                RevisionNumber = r,
            })
            .ToList();
    }
}
