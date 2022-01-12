using Hippo.Application.Common.Interfaces;
using Hippo.Application.Common.Models;
using Hippo.Core.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hippo.Application.Channels.EventHandlers;

public class ChannelDeletedEventHandler : INotificationHandler<DomainEventNotification<ChannelDeletedEvent>>
{
    private readonly ILogger<ChannelDeletedEventHandler> _logger;

    private readonly IJobScheduler _jobScheduler;

    private readonly IReverseProxy _reverseProxy;

    public ChannelDeletedEventHandler(ILogger<ChannelDeletedEventHandler> logger, IJobScheduler jobScheduler, IReverseProxy reverseProxy)
    {
        _logger = logger;
        _jobScheduler = jobScheduler;
        _reverseProxy = reverseProxy;
    }

    public Task Handle(DomainEventNotification<ChannelDeletedEvent> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;

        _logger.LogInformation("Hippo Domain Event: {DomainEvent}", domainEvent.GetType().Name);

        _reverseProxy.Stop(domainEvent.Channel);
        _jobScheduler.Stop(domainEvent.Channel);

        return Task.CompletedTask;
    }
}
