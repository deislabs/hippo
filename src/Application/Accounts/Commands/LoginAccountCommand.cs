using System.ComponentModel.DataAnnotations;
using Hippo.Application.Common.Exceptions;
using Hippo.Application.Identity;
using MediatR;

namespace Hippo.Application.Accounts.Commands;

public class LoginAccountCommand : IRequest<ApiToken>
{
    [Required]
    public string UserName { get; set; } = "";

    [Required]
    public string Password { get; set; } = "";
}

public class LoginAccountCommandHandler : IRequestHandler<LoginAccountCommand, ApiToken>
{
    private readonly IAuthenticateService _authenticateService;
    private readonly IIdentityService _identityService;
    private readonly ISignInService _signInService;

    public LoginAccountCommandHandler(IAuthenticateService authenticateService, IIdentityService identityService, ISignInService signInService)
    {
        _authenticateService = authenticateService;
        _identityService = identityService;
        _signInService = signInService;
    }

    public async Task<ApiToken> Handle(LoginAccountCommand request, CancellationToken cancellationToken)
    {
        var user = await _identityService.FindByIdAsync(request.UserName);
        if (user is null)
        {
            throw new NotFoundException(nameof(Account), request.UserName);
        }

        if ((await _signInService.PasswordSignInAsync(request.UserName, request.Password, false)).Succeeded)
        {
            return await _authenticateService.Authenticate(user, cancellationToken);
        }

        throw new LoginFailedException();
    }
}
