using Hippo.Application.Common.Interfaces;
using Hippo.Application.Common.Models;
using Hippo.Application.Rules;
using Hippo.Core.Entities;
using Hippo.Core.Enums;
using Hippo.Core.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Hippo.Application.Revisions.EventHandlers;

public class RevisionCreatedEventHandler : INotificationHandler<DomainEventNotification<RevisionCreatedEvent>>
{
    private readonly ILogger<RevisionCreatedEventHandler> _logger;

    private readonly IApplicationDbContext _context;

    public RevisionCreatedEventHandler(ILogger<RevisionCreatedEventHandler> logger, IApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task Handle(DomainEventNotification<RevisionCreatedEvent> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;

        _logger.LogInformation("Hippo Domain Event: {DomainEvent}", domainEvent.GetType().Name);

        // re-evaluate active revisions for every channel related to the same app
        var channels = await _context.Channels
            .Where(c => c.AppId == domainEvent.Revision.AppId)
            .ToListAsync(cancellationToken);

        foreach (Channel channel in channels)
        {
            if (channel.RevisionSelectionStrategy == ChannelRevisionSelectionStrategy.UseSpecifiedRevision)
            {
                var newActiveRevision = RevisionRangeRule.Parse(channel.RangeRule).Match(channel.App.Revisions);
                if (newActiveRevision != null && newActiveRevision != channel.ActiveRevision)
                {
                    channel.DomainEvents.Add(new ActiveRevisionChangedEvent(channel, newActiveRevision));
                }
                channel.ActiveRevision = newActiveRevision;
            }
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}