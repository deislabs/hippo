using Hippo.Application.Common.Interfaces;
using Hippo.Application.Common.Security;
using MediatR;

namespace Hippo.Application.Revisions.Commands;

[Authorize(Roles = "Administrator")]
[Authorize(Policy = "CanPurge")]
public class PurgeRevisionsCommand : IRequest
{
}

public class PurgeRevisionsCommandHandler : IRequestHandler<PurgeRevisionsCommand>
{
    private readonly IApplicationDbContext _context;

    public PurgeRevisionsCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(PurgeRevisionsCommand request, CancellationToken cancellationToken)
    {
        _context.Revisions.RemoveRange(_context.Revisions);

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
