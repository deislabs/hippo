using System.Text.RegularExpressions;
using FluentValidation;
using Hippo.Application.Identity;

namespace Hippo.Application.Accounts.Commands;

public class CreateAccountCommandValidator : AbstractValidator<CreateAccountCommand>
{
    private readonly IIdentityService _identityService;

    private readonly Regex validUserName = new Regex("^[a-zA-Z0-9-_]*$");

    public CreateAccountCommandValidator(IIdentityService identityService)
    {
        _identityService = identityService;

        RuleFor(a => a.UserName)
            .NotEmpty()
            .MaximumLength(64)
            .Matches(validUserName)
            .MustAsync(BeUniqueUserName).WithMessage("The specified username already exists.");

        RuleFor(a => a.Password)
            .NotEmpty()
            .MinimumLength(8);

        RuleFor(a => a.PasswordConfirm)
            .Equal(a => a.Password).WithMessage("Passwords do not match");
    }

    public async Task<bool> BeUniqueUserName(string userName, CancellationToken cancellationToken)
    {
        try
        {
            await _identityService.FindByIdAsync(userName);
            return false;
        }
        catch (Exception)
        {
            return true;
        }
    }
}
