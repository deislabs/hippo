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
            .NotEmpty().WithMessage("The username cannot be empty")
            .MaximumLength(64).WithMessage("The username cannot be longer than 64 characters")
            .Matches(validUserName).WithMessage("The username cannot contain special characters")
            .MustAsync(BeUniqueUserName).WithMessage("The specified username already exists.");
    }

    public async Task<bool> BeUniqueUserName(string userName, CancellationToken cancellationToken)
    {
        try
        {
            await _identityService.GetUserIdAsync(userName, cancellationToken);
            return false;
        }
        catch (Exception)
        {
            return true;
        }
    }
}
