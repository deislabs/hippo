using Hippo.Application.Common.Models;
using Hippo.Core.Entities;
using Hippo.Core.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hippo.Application.Channels.EventHandlers;

public class ChannelCreatedEventHandler : INotificationHandler<DomainEventNotification<CreatedEvent<Channel>>>
{
    private readonly ILogger<ChannelCreatedEventHandler> _logger;

    public ChannelCreatedEventHandler(ILogger<ChannelCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(DomainEventNotification<CreatedEvent<Channel>> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;

        _logger.LogInformation("Hippo Domain Event: {DomainEvent}", domainEvent.GetType().Name);

        // TODO: schedule a "parking lot" app instance to start serving requests

        return Task.CompletedTask;
    }
}
