using Hippo.Application.Common.Exceptions;
using Hippo.Application.Common.Interfaces;
using Hippo.Core.Entities;
using Hippo.Core.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hippo.Application.Channels.EventHandlers;

public class ChannelDeletedEventHandler : INotificationHandler<DeletedEvent<Channel>>
{
    private readonly ILogger<ChannelDeletedEventHandler> _logger;

    private readonly IJobService _jobService;

    public ChannelDeletedEventHandler(ILogger<ChannelDeletedEventHandler> logger,
        IJobService jobService)
    {
        _logger = logger;
        _jobService = jobService;
    }

    public Task Handle(DeletedEvent<Channel> notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Hippo Domain Event: {notification.GetType().Name}");

        try
        {
            _jobService.DeleteJob(jobName: notification.Entity.Id.ToString());
        }
        catch (JobFailedException e)
        {
            _logger.LogError(e.Message);
            throw;
        }

        return Task.CompletedTask;
    }
}
