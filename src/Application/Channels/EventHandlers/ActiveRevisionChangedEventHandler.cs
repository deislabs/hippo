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

    public ActiveRevisionChangedEventHandler(ILogger<ActiveRevisionChangedEventHandler> logger, IJobScheduler jobScheduler)
    {
        _logger = logger;
        _jobScheduler = jobScheduler;
    }

    public Task Handle(DomainEventNotification<ActiveRevisionChangedEvent> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;

        _logger.LogInformation("Hippo Domain Event: {DomainEvent}", domainEvent.GetType().Name);

        if (domainEvent.Channel.ActiveRevision != null)
        {
            _logger.LogInformation($"ExecuteAsync: stopping {domainEvent.Channel.App.Name} channel {domainEvent.Channel.Name} at rev {domainEvent.Channel.ActiveRevision.RevisionNumber}");
            _jobScheduler.Stop(domainEvent.Channel);
            _logger.LogInformation($"ExecuteAsync: starting {domainEvent.Channel.App.Name} channel {domainEvent.Channel.Name} at rev {domainEvent.Channel.ActiveRevision.RevisionNumber}");
            _jobScheduler.Start(domainEvent.Channel);
            _logger.LogInformation($"ExecuteAsync: started {domainEvent.Channel.App.Name} channel {domainEvent.Channel.Name} at rev {domainEvent.Channel.ActiveRevision.RevisionNumber}");
        }
        else
        {
            _logger.LogInformation($"ExecuteAsync: not restarting {domainEvent.Channel.App.Name} channel {domainEvent.Channel.Name}: no active revision");
        }

        return Task.CompletedTask;
    }
}
