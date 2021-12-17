using System.Text.RegularExpressions;
using FluentValidation;
using Hippo.Application.Common.Interfaces;

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
            .Must(BeUniqueUserName).WithMessage("The specified username already exists.");

        RuleFor(a => a.Password)
            .NotEmpty()
            .MinimumLength(8);

        RuleFor(a => a.PasswordConfirm)
            .Equal(a => a.Password).WithMessage("Passwords do not match");
    }

    public bool BeUniqueUserName(string userName)
    {
        try
        {
            _identityService.GetUserIdAsync(userName).RunSynchronously();
            return false;
        }
        catch (Exception)
        {
            return true;
        }
    }
}
