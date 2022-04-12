using Hippo.Application.Common.Interfaces;
using Hippo.Application.Common.Models;
using Hippo.Core.Entities;
using Hippo.Core.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hippo.Application.Channels.EventHandlers;

public class ActiveRevisionChangedEventHandler : INotificationHandler<DomainEventNotification<ModifiedEvent<Channel>>>
{
    private readonly ILogger<ActiveRevisionChangedEventHandler> _logger;

    private readonly IJobFactory _jobFactory;

    public ActiveRevisionChangedEventHandler(ILogger<ActiveRevisionChangedEventHandler> logger, IJobFactory jobFactory)
    {
        _logger = logger;
        _jobFactory = jobFactory;
    }

    public Task Handle(DomainEventNotification<ModifiedEvent<Channel>> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;
        Channel channel = domainEvent.Entity;

        _logger.LogInformation($"Hippo Domain Event: {domainEvent.GetType().Name}");

        if (channel.ActiveRevision is not null)
        {
            _logger.LogInformation($"{channel.App.Name}: Starting channel {channel.Name} at revision {channel.ActiveRevision.RevisionNumber}");
            var envvars = channel.EnvironmentVariables.ToDictionary(
                e => e.Key!,
                e => e.Value!
            );
            _jobFactory.Start(channel.Id, $"{channel.App.StorageId}/{channel.ActiveRevision.RevisionNumber}", envvars, channel.Domain);
            _logger.LogInformation($"Started {channel.App.Name} Channel {channel.Name} at revision {channel.ActiveRevision.RevisionNumber}");
        }
        else
        {
            _logger.LogInformation($"Not restarting {channel.App.Name} Channel {channel.Name}: no active revision");
        }

        return Task.CompletedTask;
    }
}
