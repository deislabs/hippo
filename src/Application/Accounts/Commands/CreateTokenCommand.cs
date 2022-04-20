using System.ComponentModel.DataAnnotations;
using Hippo.Application.Common.Exceptions;
using Hippo.Application.Common.Interfaces;
using MediatR;

namespace Hippo.Application.Accounts.Commands;

public class CreateTokenCommand : IRequest<TokenInfo>
{
    [Required]
    public string UserName { get; set; } = "";

    [Required]
    public string Password { get; set; } = "";
}

public class CreateTokenCommandHandler : IRequestHandler<CreateTokenCommand, TokenInfo>
{
    private readonly IIdentityService _identityService;
    private readonly ITokenService _tokenService;
    private readonly ISignInService _signInService;

    public CreateTokenCommandHandler(IIdentityService identityService, ISignInService signInService, ITokenService tokenService)
    {
        _identityService = identityService;
        _signInService = signInService;
        _tokenService = tokenService;
    }

    public async Task<TokenInfo> Handle(CreateTokenCommand request, CancellationToken cancellationToken)
    {
        var result = await _signInService.PasswordSignInAsync(request.UserName, request.Password);
        if (!result.Succeeded)
        {
            throw new LoginFailedException(result.Errors);
        }

        return _tokenService.CreateSecurityToken(await _identityService.GetUserIdAsync(request.UserName));
    }
}
