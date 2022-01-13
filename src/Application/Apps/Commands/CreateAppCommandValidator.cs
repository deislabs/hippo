using System.Text.RegularExpressions;
using FluentValidation;
using Hippo.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Hippo.Application.Apps.Commands;

public class CreateAppCommandValidator : AbstractValidator<CreateAppCommand>
{
    private readonly IApplicationDbContext _context;

    private readonly Regex validName = new Regex("^[a-zA-Z0-9-_/]*$");

    public CreateAppCommandValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(a => a.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(200)
            .Matches(validName)
            .MustAsync(BeUniqueName).WithMessage("The specified name already exists.");
    }

    public async Task<bool> BeUniqueName(CreateAppCommand model, string name, CancellationToken cancellationToken)
    {
        return await _context.Apps.AllAsync(a => a.Name != name, cancellationToken);
    }
}
