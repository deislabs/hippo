using Hippo.Application.Common.Config;
using Hippo.Application.Common.Exceptions;
using Hippo.Application.Common.Interfaces;
using Hippo.Core.Entities;
using Hippo.Core.Enums;
using Hippo.Core.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hippo.Application.Certificates.Commands;

public class CreateCertificateCommand : IRequest<Guid>
{
    public string? Name { get; set; }

    public string? PublicKey { get; set; }

    public string? PrivateKey { get; set; }

    public List<Channel>? ChannelsToBind { get; set; }
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

        entity.DomainEvents.Add(new CertificateCreatedEvent(entity));

        _context.Certificates.Add(entity);

        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
