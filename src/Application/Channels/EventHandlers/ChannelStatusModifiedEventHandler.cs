using Hippo.Application.Common.Interfaces;
using Hippo.Application.Jobs;
using Hippo.Core.Entities;
using Hippo.Core.Enums;
using Hippo.Core.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Hippo.Application.Channels.EventHandlers;

public class ChannelStatusModifiedEventHandler : INotificationHandler<ModifiedEvent<Channel>>
{
    private readonly ILogger<ChannelStatusModifiedEventHandler> _logger;
    private readonly IJobService _jobService;
    private readonly IApplicationDbContext _context;

    public ChannelStatusModifiedEventHandler(ILogger<ChannelStatusModifiedEventHandler> logger, IJobService jobService, IApplicationDbContext context)
    {
        _logger = logger;
        _jobService = jobService;
        _context = context;
    }

    public Task Handle(ModifiedEvent<Channel> notification, CancellationToken cancellationToken)
    {
        var channel = _context.Channels
            .Include(c => c.ActiveRevision)
            .Include(c => c.EnvironmentVariables)
            .Include(c => c.App)
            .First(c => c.Id == notification.Entity.Id);

        _logger.LogInformation($"Hippo Domain Event: {notification.GetType().Name}");

        if (channel.DesiredStatus == DesiredStatus.Running)
        {
            if (channel.ActiveRevision is not null)
            {
                _logger.LogInformation($"{channel.App.Name}: Starting channel {channel.Name} at revision {channel.ActiveRevision.RevisionNumber}");
                var environmentVariables = channel.EnvironmentVariables.ToDictionary(
                    e => e.Key!,
                    e => e.Value!
                );
                _jobService.StartJob(channel.Id, $"{channel.App.StorageId}/{channel.ActiveRevision.RevisionNumber}", environmentVariables, channel.Domain);
                _logger.LogInformation($"Started {channel.App.Name} Channel {channel.Name} at revision {channel.ActiveRevision.RevisionNumber}");
            }
            else
            {
                _logger.LogInformation($"Not starting {channel.App.Name} Channel {channel.Name}: no active revision");
            }
        }
        else if (channel.DesiredStatus == DesiredStatus.Dead)
        {
            _jobService.DeleteJob(channel.Id.ToString());
        }

        return Task.CompletedTask;
    }
}
