using System.Text.RegularExpressions;
using FluentValidation;

namespace Hippo.Application.EnvironmentVariables.Commands;

public class UpdateEnvironmentVariableCommandValidator : AbstractValidator<UpdateEnvironmentVariableCommand>
{
    private readonly Regex validKey = new Regex("^[a-zA-Z0-9-_]*");
    public UpdateEnvironmentVariableCommandValidator()
    {
        RuleFor(v => v.Key)
            .NotEmpty().WithMessage("Key is required.")
            .MaximumLength(32)
            .Matches(validKey);

        RuleFor(v => v.Value)
            .NotNull(); // the empty string is a valid value
    }
}
