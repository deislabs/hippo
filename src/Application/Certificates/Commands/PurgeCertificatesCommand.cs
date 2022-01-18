using Hippo.Application.Common.Interfaces;
using Hippo.Application.Common.Security;
using MediatR;

namespace Hippo.Application.Certificates.Commands;

[Authorize(Roles = "Administrator")]
[Authorize(Policy = "CanPurge")]
public class PurgeCertificatesCommand : IRequest
{
}

public class PurgeCertificatesCommandHandler : IRequestHandler<PurgeCertificatesCommand>
{
    private readonly IApplicationDbContext _context;

    public PurgeCertificatesCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(PurgeCertificatesCommand request, CancellationToken cancellationToken)
    {
        _context.Certificates.RemoveRange(_context.Certificates);

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
