using Hippo.Application.Common.Interfaces;
using Hippo.Application.Common.Models;
using Hippo.Core.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hippo.Application.Channels.EventHandlers;

public class ActiveRevisionChangedEventHandler : INotificationHandler<DomainEventNotification<ActiveRevisionChangedEvent>>
{
    private readonly ILogger<ActiveRevisionChangedEventHandler> _logger;

    private readonly IJobScheduler _jobScheduler;

    private readonly IReverseProxy _reverseProxy;

    public ActiveRevisionChangedEventHandler(ILogger<ActiveRevisionChangedEventHandler> logger, IJobScheduler jobScheduler, IReverseProxy reverseProxy)
    {
        _logger = logger;
        _jobScheduler = jobScheduler;
        _reverseProxy = reverseProxy;
    }

    public Task Handle(DomainEventNotification<ActiveRevisionChangedEvent> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;

        _logger.LogInformation("Hippo Domain Event: {DomainEvent}", domainEvent.GetType().Name);

        if (domainEvent.Channel.ActiveRevision != null)
        {
            _logger.LogInformation($"{domainEvent.Channel.App.Name}: Stopping channel {domainEvent.Channel.Name} at revision {domainEvent.Channel.ActiveRevision.RevisionNumber}");
            _jobScheduler.Stop(domainEvent.Channel);
            _logger.LogInformation($"{domainEvent.Channel.App.Name}: Starting channel {domainEvent.Channel.Name} at revision {domainEvent.Channel.ActiveRevision.RevisionNumber}");
            var status = _jobScheduler.Start(domainEvent.Channel);
            if (!status.IsRunning)
            {
                _logger.LogInformation($"{domainEvent.Channel.App.Name}: Channel {domainEvent.Channel.Name} at revision {domainEvent.Channel.ActiveRevision.RevisionNumber} failed to start");
                _reverseProxy.Stop(domainEvent.Channel);
            }
            _logger.LogInformation($"Started {domainEvent.Channel.App.Name} Channel {domainEvent.Channel.Name} at revision {domainEvent.Channel.ActiveRevision.RevisionNumber}");
            _reverseProxy.Start(domainEvent.Channel, status.ListenAddress);
        }
        else
        {
            _logger.LogInformation($"Not restarting {domainEvent.Channel.App.Name} Channel {domainEvent.Channel.Name}: no active revision");
        }

        return Task.CompletedTask;
    }
}
