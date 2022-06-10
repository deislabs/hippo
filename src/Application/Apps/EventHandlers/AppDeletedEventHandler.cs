using Hippo.Application.Common.Exceptions;
using Hippo.Application.Common.Interfaces;
using Hippo.Core.Entities;
using Hippo.Core.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hippo.Application.Apps.EventHandlers;

public class AppDeletedEventHandler : INotificationHandler<DeletedEvent<App>>
{
    private readonly ILogger<AppDeletedEventHandler> _logger;

    private readonly IJobService _jobService;

    public AppDeletedEventHandler(ILogger<AppDeletedEventHandler> logger, IJobService jobService)
    {
        _logger = logger;
        _jobService = jobService;
    }

    public Task Handle(DeletedEvent<App> notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Hippo Domain Event: {notification.GetType().Name}");

        foreach (var channel in notification.Entity.Channels)
        {
            try
            {
                _jobService.DeleteJob(jobName: channel.Id.ToString());
            }
            catch (JobFailedException e)
            {
                _logger.LogError(e.Message);
                throw;
            }
        }

        return Task.CompletedTask;
    }
}
