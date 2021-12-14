using FluentValidation;

namespace Hippo.Application.EnvironmentVariables.Commands;

public class CreateEnvironmentVariableCommandValidator : AbstractValidator<CreateEnvironmentVariableCommand>
{
    public CreateEnvironmentVariableCommandValidator()
    {
        RuleFor(v => v.Key)
            .MaximumLength(32)
            .NotEmpty();

        RuleFor(v => v.Value)
            .MaximumLength(200)
            .NotEmpty();
    }
}
