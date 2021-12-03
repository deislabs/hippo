using Hippo.Application.Common.Exceptions;
using Hippo.Application.Common.Interfaces;
using Hippo.Core.Entities;
using Hippo.Core.Events;
using MediatR;

namespace Hippo.Application.Accounts.Commands;

public class CreateAccountCommand : IRequest<string>
{
    public string? UserName { get; set; }

    public string? Password { get; set; }

    public string? PasswordConfirm { get; set; }
}

public class CreateAccountCommandHandler : IRequestHandler<CreateAccountCommand, string>
{
    private readonly IIdentityService _identityService;

    public CreateAccountCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<string> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
    {
        var result = await _identityService.CreateUserAsync(request.UserName!, request.Password!);

        return result.UserId;
    }
}
