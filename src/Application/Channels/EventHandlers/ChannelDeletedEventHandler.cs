using Hippo.Application.Common.Models;
using Hippo.Core.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hippo.Application.Channels.EventHandlers;

public class ChannelDeletedEventHandler : INotificationHandler<DomainEventNotification<ChannelDeletedEvent>>
{
    private readonly ILogger<ChannelDeletedEventHandler> _logger;

    public ChannelDeletedEventHandler(ILogger<ChannelDeletedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(DomainEventNotification<ChannelDeletedEvent> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;

        _logger.LogInformation("Hippo Domain Event: {DomainEvent}", domainEvent.GetType().Name);

        return Task.CompletedTask;
    }
}
