using Hippo.Application.Common.Interfaces;
using Hippo.Application.Common.Security;
using Hippo.Core.Entities;
using Hippo.Core.Events;
using MediatR;

namespace Hippo.Application.Revisions.Commands;

[Authorize(Roles = UserRole.Administrator)]
[Authorize(Policy = UserPolicy.CanPurge)]
public class PurgeRevisionsCommand : IRequest { }

public class PurgeRevisionsCommandHandler : IRequestHandler<PurgeRevisionsCommand>
{
    private readonly IApplicationDbContext _context;

    public PurgeRevisionsCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(PurgeRevisionsCommand request, CancellationToken cancellationToken)
    {
        foreach (var entity in _context.Revisions)
        {
            entity.AddDomainEvent(new DeletedEvent<Revision>(entity));
        }

        _context.Revisions.RemoveRange(_context.Revisions);

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
