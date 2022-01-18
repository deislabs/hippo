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

    private readonly Regex validDomainName = new Regex("^((?!-)[A-Za-z0-9-]{1, 63}(?<!-)\\.)+[A-Za-z]{2, 6}$");
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

        // TODO: validate RangeRule
    }

    public async Task<bool> BeUniqueNameForApp(UpdateChannelCommand command, string name, CancellationToken cancellationToken)
    {
        var channel = await _context.Channels.Where(c => c.Id == command.Id).SingleOrDefaultAsync(cancellationToken);
        
        if (channel == null)
        {
            throw new NotFoundException(nameof(Channel), command.Id);
        }

        return await _context.Channels.Where(c => c.AppId == channel.AppId).AllAsync(a => a.Name != name, cancellationToken);
    }
    
    public async Task<bool> BeUniqueDomainName(string domain, CancellationToken cancellationToken)
    {
        return await _context.Channels.AllAsync(a => a.Domain != domain, cancellationToken);
    }
}
