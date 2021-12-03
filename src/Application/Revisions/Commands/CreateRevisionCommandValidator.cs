using FluentValidation;

namespace Hippo.Application.Revisions.Commands;

public class CreateRevisionCommandValidator : AbstractValidator<CreateRevisionCommand>
{
    public CreateRevisionCommandValidator()
    {
        RuleFor(v => v.RevisionNumber)
            .MaximumLength(32)
            .NotEmpty();
    }
}
