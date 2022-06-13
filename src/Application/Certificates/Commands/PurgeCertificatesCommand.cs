using Hippo.Application.Common.Interfaces;
using Hippo.Application.Common.Security;
using Hippo.Core.Entities;
using Hippo.Core.Events;
using MediatR;

namespace Hippo.Application.Certificates.Commands;

[Authorize(Roles = UserRole.Administrator)]
[Authorize(Policy = UserPolicy.CanPurge)]
public class PurgeCertificatesCommand : IRequest { }

public class PurgeCertificatesCommandHandler : IRequestHandler<PurgeCertificatesCommand>
{
    private readonly IApplicationDbContext _context;

    public PurgeCertificatesCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(PurgeCertificatesCommand request, CancellationToken cancellationToken)
    {
        foreach (var entity in _context.Certificates)
        {
            entity.AddDomainEvent(new DeletedEvent<Certificate>(entity));
        }

        _context.Certificates.RemoveRange(_context.Certificates);

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
