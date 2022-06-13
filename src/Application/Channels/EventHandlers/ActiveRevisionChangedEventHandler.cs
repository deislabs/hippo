using Hippo.Application.Common.Interfaces;
using Hippo.Core.Entities;
using Hippo.Core.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hippo.Application.Channels.EventHandlers;

public class ActiveRevisionChangedEventHandler : INotificationHandler<ModifiedEvent<Channel>>
{
    private readonly ILogger<ActiveRevisionChangedEventHandler> _logger;

    private readonly IJobService _jobService;

    public ActiveRevisionChangedEventHandler(ILogger<ActiveRevisionChangedEventHandler> logger,
        IJobService jobService)
    {
        _logger = logger;
        _jobService = jobService;
    }

    public Task Handle(ModifiedEvent<Channel> notification, CancellationToken cancellationToken)
    {
        Channel channel = notification.Entity;

        _logger.LogInformation($"Hippo Domain Event: {notification.GetType().Name}");

        if (channel.ActiveRevision is not null)
        {
            _logger.LogInformation($"{channel.App.Name}: Starting channel {channel.Name} at revision {channel.ActiveRevision.RevisionNumber}");
            var envvars = channel.EnvironmentVariables.ToDictionary(
                e => e.Key!,
                e => e.Value!
            );
            _jobService.StartJob(channel.Id, $"{channel.App.StorageId}/{channel.ActiveRevision.RevisionNumber}", envvars, channel.Domain);
            _logger.LogInformation($"Started {channel.App.Name} Channel {channel.Name} at revision {channel.ActiveRevision.RevisionNumber}");
        }
        else
        {
            _logger.LogInformation($"Not restarting {channel.App.Name} Channel {channel.Name}: no active revision");
        }

        return Task.CompletedTask;
    }
}
