using Hippo.Application.Common.Interfaces;
using Hippo.Application.Common.Security;
using MediatR;

namespace Hippo.Application.EnvironmentVariables.Commands;

[Authorize(Roles = "Administrator")]
[Authorize(Policy = "CanPurge")]
public class PurgeEnvironmentVariablesCommand : IRequest
{
}

public class PurgeEnvironmentVariablesCommandHandler : IRequestHandler<PurgeEnvironmentVariablesCommand>
{
    private readonly IApplicationDbContext _context;

    public PurgeEnvironmentVariablesCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(PurgeEnvironmentVariablesCommand request, CancellationToken cancellationToken)
    {
        _context.EnvironmentVariables.RemoveRange(_context.EnvironmentVariables);

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
