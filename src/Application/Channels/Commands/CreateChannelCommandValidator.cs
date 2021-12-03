using FluentValidation;

namespace Hippo.Application.Channels.Commands;

public class CreateChannelCommandValidator : AbstractValidator<CreateChannelCommand>
{
    public CreateChannelCommandValidator()
    {
        RuleFor(v => v.Name)
            .MaximumLength(32)
            .NotEmpty();
    }
}
