using System.ComponentModel.DataAnnotations;
using Hippo.Application.Common.Exceptions;
using Hippo.Application.Common.Interfaces;
using Hippo.Core.Entities;
using Hippo.Core.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hippo.Application.Revisions.Commands;

public class DeleteRevisionCommand : IRequest
{
    [Required]
    public Guid Id { get; set; }
}

public class DeleteRevisionCommandHandler : IRequestHandler<DeleteRevisionCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteRevisionCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(DeleteRevisionCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Revisions
            .Where(l => l.Id == request.Id)
            .SingleOrDefaultAsync(cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException(nameof(Revision), request.Id);
        }

        entity.DomainEvents.Add(new DeletedEvent<Revision>(entity));

        _context.Revisions.Remove(entity);

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
