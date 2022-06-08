using Hippo.Application.Common.Interfaces;
using Hippo.Application.Common.Security;
using Hippo.Core.Entities;
using Hippo.Core.Events;
using MediatR;

namespace Hippo.Application.EnvironmentVariables.Commands;

[Authorize(Roles = UserRole.Administrator)]
[Authorize(Policy = UserPolicy.CanPurge)]
public class PurgeEnvironmentVariablesCommand : IRequest { }

public class PurgeEnvironmentVariablesCommandHandler : IRequestHandler<PurgeEnvironmentVariablesCommand>
{
    private readonly IApplicationDbContext _context;

    public PurgeEnvironmentVariablesCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(PurgeEnvironmentVariablesCommand request, CancellationToken cancellationToken)
    {
        foreach (EnvironmentVariable environmentVariable in _context.EnvironmentVariables)
        {
            environmentVariable.DomainEvents.Add(new DeletedEvent<EnvironmentVariable>(environmentVariable));
        }
        _context.EnvironmentVariables.RemoveRange(_context.EnvironmentVariables);

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
