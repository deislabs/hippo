using FluentValidation;

namespace Hippo.Application.Apps.Commands;

public class CreateAppCommandValidator : AbstractValidator<CreateAppCommand>
{
    public CreateAppCommandValidator()
    {
        RuleFor(v => v.Name)
            .MaximumLength(32)
            .NotEmpty();

        RuleFor(v => v.StorageId)
            .NotEmpty();
    }
}
