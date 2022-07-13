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
    public string UserName { get; set; } = string.Empty;
}

public class CreateAccountCommandHandler : IRequestHandler<CreateAccountCommand, string>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IIdentityService _identityService;
    private readonly HippoConfig _hippoConfig;

    public CreateAccountCommandHandler(ICurrentUserService currentUserService, IIdentityService identityService, HippoConfig hippoConfig)
    {
        _currentUserService = currentUserService;
        _identityService = identityService;
        _hippoConfig = hippoConfig;
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
                    if (string.IsNullOrEmpty(_currentUserService.UserId) || !await _identityService.IsInRoleAsync(_currentUserService.UserId, UserRole.Administrator, cancellationToken))
                    {
                        throw new ForbiddenAccessException("only administrators are allowed to register new accounts");
                    }
                    break;
                }
        }

        var result = await _identityService.CreateUserAsync(request.UserName, cancellationToken);
        if (!result.Result.Succeeded)
        {
            throw new AccountException(result.Result.Errors);
        }

        return result.UserId;
    }
}
