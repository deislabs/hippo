using Hippo.Application.Common.Exceptions;
using Hippo.Application.Common.Interfaces;
using Hippo.Application.Common.Models;
using Hippo.Application.Jobs;
using Hippo.Core.Entities;
using Hippo.Core.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hippo.Application.Channels.EventHandlers;

public class ChannelDeletedEventHandler : INotificationHandler<DomainEventNotification<DeletedEvent<Channel>>>
{
    private readonly ILogger<ChannelDeletedEventHandler> _logger;

    private readonly JobScheduler _jobScheduler = JobScheduler.Current;

    private readonly INomadService _nomadService;

    public ChannelDeletedEventHandler(ILogger<ChannelDeletedEventHandler> logger,
        INomadService nomadService)
    {
        _logger = logger;
        _nomadService = nomadService;
    }

    public Task Handle(DomainEventNotification<DeletedEvent<Channel>> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;

        _logger.LogInformation($"Hippo Domain Event: {domainEvent.GetType().Name}");

        try
        {
            _nomadService.DeleteJob(domainEvent.Entity.Id.ToString());
        }
        catch (JobFailedException e)
        {
            _logger.LogError(e.Message);
            throw;
        }

        return Task.CompletedTask;
    }
}
