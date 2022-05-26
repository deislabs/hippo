using System.ComponentModel.DataAnnotations;
using Hippo.Application.Common.Config;
using Hippo.Application.Common.Exceptions;
using Hippo.Application.Common.Interfaces;
using Hippo.Application.Common.Security;
using Hippo.Core.Enums;
using MediatR;

namespace Hippo.Application.Accounts.Commands;

public class CreateAccountCommand : IRequest<string>
{
    [Required]
    public string UserName { get; set; } = "";

    [Required]
    public string Password { get; set; } = "";
}

public class CreateAccountCommandHandler : IRequestHandler<CreateAccountCommand, string>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly HippoConfig _hippoConfig;
    private readonly IIdentityService _identityService;

    public CreateAccountCommandHandler(ICurrentUserService currentUserService, HippoConfig hippoConfig, IIdentityService identityService)
    {
        _currentUserService = currentUserService;
        _hippoConfig = hippoConfig;
        _identityService = identityService;
    }

    public async Task<string> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
    {
        switch (_hippoConfig.RegistrationMode)
        {
            case RegistrationMode.Closed:
                {
                    throw new ForbiddenAccessException("registration closed");
                }

            case RegistrationMode.AdministratorOnly:
                {
                    if (_currentUserService.UserId is null || !await _identityService.IsInRoleAsync(_currentUserService.UserId, UserRole.Administrator))
                    {
                        throw new ForbiddenAccessException("only administrators are allowed to register new accounts");
                    }
                    break;
                }
        }

        var result = await _identityService.CreateUserAsync(request.UserName, request.Password);
        return result.UserId;
    }
}
