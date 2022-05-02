using System.Text.RegularExpressions;
using FluentValidation;
using Hippo.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Hippo.Application.Apps.Commands;

public class CreateAppCommandValidator : AbstractValidator<CreateAppCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IBindleService _bindleService;

    private readonly Regex validName = new Regex("^[a-zA-Z0-9-_]*$");

    private readonly Regex validStorageId = new Regex("^[a-zA-Z0-9-_./]*$");

    public CreateAppCommandValidator(IApplicationDbContext context,
        IBindleService bindleService)
    {
        _context = context;
        _bindleService = bindleService;

        RuleFor(a => a.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(128)
            .Matches(validName)
            .MustAsync(BeUniqueName).WithMessage("The specified name already exists.");

        RuleFor(a => a.StorageId)
            .NotEmpty().WithMessage("Storage ID is required.")
            .MustAsync(BeValidStorageId).WithMessage("Storage ID not found")
            .MaximumLength(200)
            .Matches(validStorageId);

    }

    public async Task<bool> BeUniqueName(CreateAppCommand model, string name, CancellationToken cancellationToken)
    {
        return await _context.Apps.AllAsync(a => a.Name != name, cancellationToken);
    }

    public async Task<bool> BeValidStorageId(CreateAppCommand model, string storageId, CancellationToken cancellationToken)
    {
        var storages = await _bindleService.QueryAvailableStorages(storageId, 0, null);

        return storages.Contains(storageId);
    }
}
