using FluentValidation;

namespace Hippo.Application.Revisions.Commands;

public class CreateRevisionCommandValidator : AbstractValidator<CreateRevisionCommand>
{
    public CreateRevisionCommandValidator()
    {
        // NOTE: no semver compliance is required; users can opt out of automatic revision
        // selection strategy (or we could allow alternative version sorting strategies?)
        RuleFor(v => v.RevisionNumber)
            .NotEmpty()
            .MaximumLength(256);
    }
}
