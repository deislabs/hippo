using Hippo.Application.Common.Interfaces;
using Hippo.Application.Common.Models;
using Hippo.Application.Jobs;
using Hippo.Core.Entities;
using Hippo.Core.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hippo.Application.Channels.EventHandlers;

public class ActiveRevisionChangedEventHandler : INotificationHandler<DomainEventNotification<ActiveRevisionChangedEvent>>
{
    private readonly ILogger<ActiveRevisionChangedEventHandler> _logger;

    private readonly IJobFactory _jobFactory;

    private readonly JobScheduler _jobScheduler = JobScheduler.Current;

    public ActiveRevisionChangedEventHandler(ILogger<ActiveRevisionChangedEventHandler> logger, IJobFactory jobFactory)
    {
        _logger = logger;
        _jobFactory = jobFactory;
    }

    public Task Handle(DomainEventNotification<ActiveRevisionChangedEvent> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;
        Channel channel = domainEvent.Channel;

        _logger.LogInformation("Hippo Domain Event: {DomainEvent}", domainEvent.GetType().Name);

        if (channel.ActiveRevision != null)
        {
            _logger.LogInformation($"{channel.App.Name}: Stopping channel {channel.Name} at revision {channel.ActiveRevision.RevisionNumber}");
            foreach (var j in _jobScheduler.GetRunningJobs())
            {
                if (j.Id == channel.Id)
                {
                    j.Stop();
                }
            }
            _logger.LogInformation($"{channel.App.Name}: Starting channel {channel.Name} at revision {channel.ActiveRevision.RevisionNumber}");
            var envvars = channel.EnvironmentVariables
                .ToDictionary(
                    e => e.Key!,
                    e => e.Value!
                );
            var job = _jobFactory.StartNew(channel.Id, $"{channel.App.StorageId}/{channel.ActiveRevision.RevisionNumber}", envvars, channel.Domain);
            if (!job.IsRunning)
            {
                _logger.LogError($"{channel.App.Name}: Channel {channel.Name} at revision {channel.ActiveRevision.RevisionNumber} failed to start");
            }
            _logger.LogInformation($"Started {channel.App.Name} Channel {channel.Name} at revision {channel.ActiveRevision.RevisionNumber}");
        }
        else
        {
            _logger.LogInformation($"Not restarting {channel.App.Name} Channel {channel.Name}: no active revision");
        }

        return Task.CompletedTask;
    }
}
