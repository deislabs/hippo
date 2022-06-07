using System.Text.RegularExpressions;
using FluentValidation;
using Hippo.Application.Common.Exceptions;
using Hippo.Application.Common.Interfaces;
using Hippo.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Hippo.Application.Channels.Commands;

public class UpdateChannelCommandValidator : AbstractValidator<UpdateChannelCommand>
{
    private readonly Regex validName = new Regex("^[a-zA-Z0-9-_]*$");

    private readonly Regex validDomainName = new Regex(@"^([a-z0-9]+(-[a-z0-9]+)*\.)+[a-z]{2,}$");
    private readonly IApplicationDbContext _context;

    public UpdateChannelCommandValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(v => v.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(64)
            .Matches(validName)
            .MustAsync(BeUniqueNameForApp).WithMessage("A channel with the same name already exists for this app.");

        RuleFor(v => v.Domain)
            .NotEmpty().WithMessage("Domain is required.")
            .Matches(validDomainName)
            .MustAsync(BeUniqueDomainName).WithMessage("The specified domain already exists.");

        RuleFor(v => v.RangeRule)
            .NotEqual("").WithMessage("Range rule cannot be an empty string.");

        // TODO: validate RangeRule syntax
    }

    public async Task<bool> BeUniqueNameForApp(UpdateChannelCommand command, string name, CancellationToken cancellationToken)
    {
        var channel = await _context.Channels.Where(c => c.Id == command.Id).SingleOrDefaultAsync(cancellationToken);

        if (channel is null)
        {
            throw new NotFoundException(nameof(Channel), command.Id);
        }

        return await _context.Channels.Where(c => c.AppId == channel.AppId).AllAsync(a => a.Name != name, cancellationToken);
    }

    public async Task<bool> BeUniqueDomainName(UpdateChannelCommand command, string domain, CancellationToken cancellationToken)
    {
        return await _context.Channels.Where(c => c.Id != command.Id).AllAsync(a => a.Domain != domain, cancellationToken);
    }
}
