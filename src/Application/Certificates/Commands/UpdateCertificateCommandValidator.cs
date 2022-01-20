using System.Text.RegularExpressions;
using FluentValidation;
using Hippo.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Hippo.Application.Certificates.Commands;

public class UpdateCertificateCommandValidator : AbstractValidator<UpdateCertificateCommand>
{
    private readonly Regex validName = new Regex("^[a-zA-Z0-9-_]*$");

    private readonly IApplicationDbContext _context;

    public UpdateCertificateCommandValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(v => v.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(64)
            .Matches(validName)
            .MustAsync(BeUniqueName).WithMessage("A certificate with the same name already exists.");

        RuleFor(v => v.PublicKey)
            .NotEmpty().WithMessage("Public key is required.");

        RuleFor(v => v.PrivateKey)
            .NotEmpty().WithMessage("Private key is required.");

        // TODO: validate public/private key as valid x.509 certificate keypair
    }

    public async Task<bool> BeUniqueName(string name, CancellationToken cancellationToken)
    {
        return await _context.Certificates.AllAsync(c => c.Name != name, cancellationToken);
    }
}
