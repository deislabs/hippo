using Hippo.Application.Common.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace Hippo.Application.Apps.Commands;

public class UpdateAppCommandValidator : AbstractValidator<UpdateAppCommand>
{
    private readonly IApplicationDbContext _context;

    private readonly Regex validName = new Regex("^[a-zA-Z0-9-_]*$");

    private readonly Regex validStorageId = new Regex("^[a-zA-Z0-9-_/]*$");

    public UpdateAppCommandValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(a => a.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(128)
            .Matches(validName)
            .Must(BeUniqueName).WithMessage("The specified name already exists.");

        RuleFor(a => a.StorageId)
            .NotEmpty()
            .MaximumLength(200)
            .Matches(validStorageId);
    }

    public bool BeUniqueName(UpdateAppCommand model, string name)
    {
        return _context.Apps.All(a => a.Name != name);
    }
}
