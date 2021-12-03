using FluentValidation;
using Hippo.Application.Common.Interfaces;

namespace Hippo.Application.Accounts.Commands;

public class CreateAccountCommandValidator : AbstractValidator<CreateAccountCommand>
{
    private readonly IIdentityService _identityService;
    public CreateAccountCommandValidator(IIdentityService identityService)
    {
        _identityService = identityService;

        RuleFor(a => a.UserName)
            .MaximumLength(32).WithMessage("username must not exceed 32 characters.")
            .MustAsync(BeUniqueUserName).WithMessage("username already exists.")
            .NotEmpty();

        RuleFor(a => a.Password)
            .MinimumLength(8)
            .NotEmpty();

        RuleFor(a => a.PasswordConfirm)
            .Equal(a => a.Password).WithMessage("Passwords do not match");
    }

    public async Task<bool> BeUniqueUserName(string userName, CancellationToken cancellationToken)
    {
        try
        {
            await _identityService.GetUserIdAsync(userName);
            return false;
        }
        catch (Exception)
        {
            return true;
        }
    }
}
