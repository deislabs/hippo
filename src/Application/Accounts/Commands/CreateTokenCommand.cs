using Hippo.Application.Common.Exceptions;
using Hippo.Application.Common.Interfaces;
using Hippo.Core.Entities;
using Hippo.Core.Events;
using MediatR;

namespace Hippo.Application.Accounts.Commands;

public class CreateTokenCommand : IRequest<TokenInfo>
{
    public string UserName { get; set; }

    public string Password { get; set; }

    public CreateTokenCommand(string username, string password)
    {
        UserName = username;
        Password = password;
    }
}

public class CreateTokenCommandHandler : IRequestHandler<CreateTokenCommand, TokenInfo>
{
    private readonly IIdentityService _identityService;
    private readonly ITokenService _tokenService;

    public CreateTokenCommandHandler(IIdentityService identityService, ITokenService tokenService)
    {
        _identityService = identityService;
        _tokenService = tokenService;
    }

    public async Task<TokenInfo> Handle(CreateTokenCommand request, CancellationToken cancellationToken)
    {
        if (await _identityService.CheckPasswordAsync(request.UserName, request.Password))
        {
            return _tokenService.CreateSecurityToken(request.UserName);
        }

        throw new LoginFailedException();
    }
}
