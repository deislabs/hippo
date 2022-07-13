using FluentValidation;

namespace Hippo.Application.Accounts.Commands;

public class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordCommandValidator()
    {
        RuleFor(a => a.Password)
            .NotEmpty().WithMessage("The password cannot be empty")
            .MinimumLength(6).WithMessage("The password must be at least 6 characters long");
    }
}
