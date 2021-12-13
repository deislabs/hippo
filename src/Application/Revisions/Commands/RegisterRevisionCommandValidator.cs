using FluentValidation;

namespace Hippo.Application.Revisions.Commands;

public class RegisterRevisionCommandValidator : AbstractValidator<RegisterRevisionCommand>
{
    public RegisterRevisionCommandValidator()
    {
        RuleFor(v => v.RevisionNumber)
            .MaximumLength(32)
            .NotEmpty();
    }
}
