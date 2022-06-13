using System.ComponentModel.DataAnnotations;
using Hippo.Application.Common.Exceptions;
using Hippo.Application.Common.Interfaces;
using Hippo.Core.Entities;
using Hippo.Core.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hippo.Application.Apps.Commands;

public class DeleteAppCommand : IRequest
{
    [Required]
    public Guid Id { get; set; }
}

public class DeleteAppCommandHandler : IRequestHandler<DeleteAppCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteAppCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(DeleteAppCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Apps
            .Where(a => a.Id == request.Id)
            .Include(a => a.Channels)
            .SingleOrDefaultAsync(cancellationToken);

        if (entity is null)
        {
            throw new NotFoundException(nameof(App), request.Id);
        }

        entity.AddDomainEvent(new DeletedEvent<App>(entity));

        _context.Apps.Remove(entity);

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
