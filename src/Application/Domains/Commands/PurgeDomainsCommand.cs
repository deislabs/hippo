using Hippo.Application.Common.Interfaces;
using Hippo.Application.Common.Security;
using MediatR;

namespace Hippo.Application.Domains.Commands;

[Authorize(Roles = "Administrator")]
[Authorize(Policy = "CanPurge")]
public class PurgeDomainsCommand : IRequest
{
}

public class PurgeDomainsCommandHandler : IRequestHandler<PurgeDomainsCommand>
{
    private readonly IApplicationDbContext _context;

    public PurgeDomainsCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(PurgeDomainsCommand request, CancellationToken cancellationToken)
    {
        _context.Domains.RemoveRange(_context.Domains);

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
