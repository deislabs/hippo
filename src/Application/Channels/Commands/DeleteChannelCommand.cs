using Hippo.Application.Common.Exceptions;
using Hippo.Application.Common.Interfaces;
using Hippo.Core.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hippo.Application.Channels.Commands;

public class DeleteChannelCommand : IRequest
{
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

        if (entity == null)
        {
            throw new NotFoundException(nameof(Channel), request.Id);
        }

        _context.Channels.Remove(entity);

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
