using System.ComponentModel.DataAnnotations;
using Hippo.Application.Common.Exceptions;
using Hippo.Application.Common.Interfaces;
using MediatR;

namespace Hippo.Application.Accounts.Commands;

public class LoginWithPasswordCommand : IRequest<TokenInfo>
{
    [Required]
    public string UserName { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}

public class LoginWithPasswordCommandHandler : IRequestHandler<LoginWithPasswordCommand, TokenInfo>
{
    private readonly IIdentityService _identityService;
    private readonly ITokenService _tokenService;
    private readonly ISignInService _signInService;

    public LoginWithPasswordCommandHandler(IIdentityService identityService, ISignInService signInService, ITokenService tokenService)
    {
        _identityService = identityService;
        _signInService = signInService;
        _tokenService = tokenService;
    }

    public async Task<TokenInfo> Handle(LoginWithPasswordCommand request, CancellationToken cancellationToken)
    {
        var id = await _identityService.GetUserIdAsync(request.UserName, cancellationToken);
        var result = await _signInService.PasswordSignInAsync(id, request.Password);
        if (!result.Succeeded)
        {
            throw new LoginFailedException(result.Errors);
        }

        return _tokenService.CreateSecurityToken(id);
    }
}
