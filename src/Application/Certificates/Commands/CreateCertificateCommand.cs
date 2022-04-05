using System.ComponentModel.DataAnnotations;
using Hippo.Application.Common.Interfaces;
using Hippo.Core.Entities;
using Hippo.Core.Events;
using MediatR;

namespace Hippo.Application.Certificates.Commands;

public class CreateCertificateCommand : IRequest<Guid>
{
    [Required]
    public string Name { get; set; } = "";

    [Required]
    public string PublicKey { get; set; } = "";

    [Required]
    public string PrivateKey { get; set; } = "";
}

public class CreateCertificateCommandHandler : IRequestHandler<CreateCertificateCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public CreateCertificateCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateCertificateCommand request, CancellationToken cancellationToken)
    {
        var entity = new Certificate
        {
            Name = request.Name,
            PublicKey = request.PublicKey,
            PrivateKey = request.PrivateKey,
        };

        entity.DomainEvents.Add(new CreatedEvent<Certificate>(entity));

        _context.Certificates.Add(entity);

        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
