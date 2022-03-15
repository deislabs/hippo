using System.Text.RegularExpressions;
using FluentValidation;

namespace Hippo.Application.Revisions.Commands;

public class RegisterRevisionCommandValidator : AbstractValidator<RegisterRevisionCommand>
{
    private readonly Regex validStorageId = new Regex("^[a-zA-Z0-9-_/]*");

    public RegisterRevisionCommandValidator()
    {
        RuleFor(v => v.AppStorageId)
            .NotEmpty().WithMessage("Storage ID is required.")
            .MaximumLength(200)
            .Matches(validStorageId);

        RuleFor(v => v.RevisionNumber)
            .NotEmpty()
            .MaximumLength(128);
    }
}
