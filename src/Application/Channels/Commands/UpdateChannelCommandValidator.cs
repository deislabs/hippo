using System.Text.RegularExpressions;
using FluentValidation;
using Hippo.Application.Common.Interfaces;
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
            .Matches(validName);

        RuleFor(v => v.Domain)
            .NotEmpty().WithMessage("Domain is required.")
            .Matches(validDomainName)
            .Must(BeUniqueDomainName).WithMessage("The specified domain already exists.");

        // TODO: validate RangeRule
    }

    public bool BeUniqueDomainName(string domain)
    {
        return _context.Channels.All(a => a.Domain != domain);
    }
}
