using System.ComponentModel.DataAnnotations;
using Hippo.Application.Identity;
using MediatR;

namespace Hippo.Application.Accounts.Commands;

public class CreateAccountCommand : IRequest<string>
{
    [Required]
    public string UserName { get; set; } = "";

    [Required]
    public string Password { get; set; } = "";

    [Required]
    public string PasswordConfirm { get; set; } = "";
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
        var result = await _identityService.CreateUserAsync(request.UserName, request.Password);

        return result.UserId;
    }
}
