using System.Text.RegularExpressions;
using FluentValidation;

namespace Hippo.Application.EnvironmentVariables.Commands;

public class CreateEnvironmentVariableCommandValidator : AbstractValidator<CreateEnvironmentVariableCommand>
{
    private readonly Regex validKey = new Regex("^[a-zA-Z0-9-_]*");
    public CreateEnvironmentVariableCommandValidator()
    {
        RuleFor(v => v.Key)
            .NotEmpty().WithMessage("Key is required.")
            .MaximumLength(32)
            .Matches(validKey);

        RuleFor(v => v.Value)
            .NotNull(); // the empty string is a valid value
    }
}
