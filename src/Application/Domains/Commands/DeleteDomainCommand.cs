using Hippo.Application.Common.Exceptions;
using Hippo.Application.Common.Interfaces;
using Hippo.Core.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hippo.Application.Domains.Commands;

public class DeleteDomainCommand : IRequest
{
    public Guid Id { get; set; }
}

public class DeleteDomainCommandHandler : IRequestHandler<DeleteDomainCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteDomainCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(DeleteDomainCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Domains
            .Where(l => l.Id == request.Id)
            .SingleOrDefaultAsync(cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException(nameof(Domain), request.Id);
        }

        _context.Domains.Remove(entity);

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
