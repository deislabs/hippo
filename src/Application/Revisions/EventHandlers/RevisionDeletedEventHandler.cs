using Hippo.Application.Common.Interfaces;
using Hippo.Application.Rules;
using Hippo.Core.Entities;
using Hippo.Core.Enums;
using Hippo.Core.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Hippo.Application.Revisions.EventHandlers;

public class RevisionDeletedEventHandler : INotificationHandler<DeletedEvent<Revision>>
{
    private readonly ILogger<RevisionCreatedEventHandler> _logger;

    private readonly IApplicationDbContext _context;

    public RevisionDeletedEventHandler(ILogger<RevisionCreatedEventHandler> logger, IApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task Handle(DeletedEvent<Revision> notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Hippo Domain Event: {notification.GetType().Name}");

        // re-evaluate active revisions for every channel related to the same app
        var channels = await _context.Channels
            .Where(c => c.AppId == notification.Entity.AppId)
            .ToListAsync(cancellationToken);

        foreach (Channel channel in channels)
        {
            if (channel.RevisionSelectionStrategy == ChannelRevisionSelectionStrategy.UseRangeRule)
            {
                var activeRevision = RevisionRangeRule.Parse(channel.RangeRule).Match(channel.App.Revisions);
                if (activeRevision is not null && activeRevision != channel.ActiveRevision)
                {
                    _logger.LogInformation($"Channel {channel.Id} changed its active revision to {activeRevision.Id}");
                    channel.ActiveRevision = activeRevision;
                    channel.AddDomainEvent(new ModifiedEvent<Channel>(channel));
                }
            }
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}
