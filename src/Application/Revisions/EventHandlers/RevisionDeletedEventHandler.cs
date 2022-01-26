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

public class RevisionDeletedEventHandler : INotificationHandler<DomainEventNotification<RevisionDeletedEvent>>
{
    private readonly ILogger<RevisionCreatedEventHandler> _logger;

    private readonly IApplicationDbContext _context;

    public RevisionDeletedEventHandler(ILogger<RevisionCreatedEventHandler> logger, IApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task Handle(DomainEventNotification<RevisionDeletedEvent> notification, CancellationToken cancellationToken)
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
                channel.ActiveRevision = RevisionRangeRule.Parse(channel.RangeRule).Match(channel.App.Revisions);
            }
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}
