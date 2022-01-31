using Hippo.Application.Common.Interfaces;
using Hippo.Application.Common.Security;
using Hippo.Core.Entities;
using Hippo.Core.Events;
using MediatR;

namespace Hippo.Application.Channels.Commands;

[Authorize(Roles = "Administrator")]
[Authorize(Policy = "CanPurge")]
public class PurgeChannelsCommand : IRequest
{
}

public class PurgeChannelsCommandHandler : IRequestHandler<PurgeChannelsCommand>
{
    private readonly IApplicationDbContext _context;

    public PurgeChannelsCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(PurgeChannelsCommand request, CancellationToken cancellationToken)
    {
        foreach (Channel channel in _context.Channels)
        {
            channel.DomainEvents.Add(new ChannelDeletedEvent(channel));
        }
        _context.Channels.RemoveRange(_context.Channels);

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
