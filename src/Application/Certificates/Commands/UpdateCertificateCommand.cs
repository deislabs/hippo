using System.ComponentModel.DataAnnotations;
using Hippo.Application.Common.Exceptions;
using Hippo.Application.Common.Interfaces;
using Hippo.Core.Entities;
using MediatR;

namespace Hippo.Application.Certificates.Commands;

public class UpdateCertificateCommand : IRequest
{
    [Required]
    public Guid Id { get; set; }

    [Required]
    public string Name { get; set; } = "";

    [Required]
    public string PublicKey { get; set; } = "";

    [Required]
    public string PrivateKey { get; set; } = "";
}

public class UpdateCertificateCommandHandler : IRequestHandler<UpdateCertificateCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateCertificateCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(UpdateCertificateCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Certificates
            .FindAsync(new object[] { request.Id }, cancellationToken);

        if (entity is null)
        {
            throw new NotFoundException(nameof(Certificate), request.Id);
        }

        entity.Name = request.Name;
        entity.PublicKey = request.PublicKey;
        entity.PrivateKey = request.PrivateKey;

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
