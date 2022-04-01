using Hippo.Application.Identity;
using MediatR;

namespace Hippo.Application.Accounts.Commands;

public class LogoutAccountCommand : IRequest { }

public class LogoutAccountCommandHandler : IRequestHandler<LogoutAccountCommand>
{
    private readonly ISignInService _signInService;

    public LogoutAccountCommandHandler(ISignInService signInService)
    {
        _signInService = signInService;
    }

    public async Task<Unit> Handle(LogoutAccountCommand request, CancellationToken cancellationToken)
    {
        return await _signInService.SignOutAsync();
    }
}
