using Hippo.Application.Common.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Hippo.Application.Apps.Commands;

public class UpdateAppCommandValidator : AbstractValidator<UpdateAppCommand>
{
    private readonly IApplicationDbContext _context;
    public UpdateAppCommandValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(v => v.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(32).WithMessage("Name must not exceed 32 characters.")
            .MustAsync(BeUniqueName).WithMessage("The specified name already exists.");

        RuleFor(v => v.StorageId)
            .NotEmpty();
    }

    public async Task<bool> BeUniqueName(UpdateAppCommand model, string name, CancellationToken cancellationToken)
    {
        return await _context.Apps
            .Where(l => l.Id != model.Id)
            .AllAsync(l => l.Name != name, cancellationToken);
    }
}
