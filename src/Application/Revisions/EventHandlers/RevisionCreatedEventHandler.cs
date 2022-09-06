using Hippo.Application.Channels.Commands;
using Hippo.Application.Common.Interfaces;
using Hippo.Application.Rules;
using Hippo.Core.Entities;
using Hippo.Core.Enums;
using Hippo.Core.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Hippo.Application.Revisions.EventHandlers;

public class RevisionCreatedEventHandler : INotificationHandler<CreatedEvent<Revision>>
{
    private readonly ILogger<RevisionCreatedEventHandler> _logger;

    private readonly IApplicationDbContext _context;

    private readonly IMediator _mediator;

    public RevisionCreatedEventHandler(ILogger<RevisionCreatedEventHandler> logger,
        IApplicationDbContext context,
        IMediator mediator)
    {
        _logger = logger;
        _context = context;
        _mediator = mediator;
    }

    public async Task Handle(CreatedEvent<Revision> notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Hippo Domain Event: {notification.GetType().Name}");

        // re-evaluate active revisions for every channel related to the same app
        var channels = await _context.Channels
            .Where(c => c.AppId == notification.Entity.AppId)
            .Include(c => c.EnvironmentVariables)
            .ToListAsync(cancellationToken);

        foreach (Channel channel in channels)
        {
            if (channel.RevisionSelectionStrategy == ChannelRevisionSelectionStrategy.UseRangeRule)
            {
                var activeRevision = RevisionRangeRule.Parse(channel.RangeRule).Match(channel.App.Revisions);
                if (activeRevision is not null && activeRevision != channel.ActiveRevision && activeRevision.RevisionNumber == notification.Entity.RevisionNumber)
                {
                    _logger.LogInformation($"Channel {channel.Id} changed its active revision to {activeRevision.Id}");
                    
                    await _mediator.Send(new UpdateChannelCommand
                    {
                        Id = channel.Id,
                        ActiveRevisionId = activeRevision.Id,
                        LastPublishDate = DateTime.UtcNow,
                        CertificateId = channel.CertificateId,
                        Domain = channel.Domain,
                        Name = channel.Name,
                        RangeRule = channel.RangeRule,
                        RevisionSelectionStrategy = channel.RevisionSelectionStrategy,
                    }, cancellationToken);
                }
            }
        }
    }
}
