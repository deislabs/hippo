using System.ComponentModel.DataAnnotations;
using Hippo.Application.Common.Exceptions;
using Hippo.Application.Common.Interfaces;
using Hippo.Core.Entities;
using Hippo.Core.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hippo.Application.Channels.Commands;

public class DeleteChannelCommand : IRequest
{
    [Required]
    public Guid Id { get; set; }
}

public class DeleteChannelCommandHandler : IRequestHandler<DeleteChannelCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteChannelCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(DeleteChannelCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Channels
            .Where(l => l.Id == request.Id)
            .SingleOrDefaultAsync(cancellationToken);

        if (entity is null)
        {
            throw new NotFoundException(nameof(Channel), request.Id);
        }

        entity.AddDomainEvent(new DeletedEvent<Channel>(entity));

        _context.Channels.Remove(entity);

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
