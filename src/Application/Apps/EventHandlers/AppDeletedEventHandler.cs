using Hippo.Application.Common.Models;
using Hippo.Core.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hippo.Application.Apps.EventHandlers;

public class AppDeletedEventHandler : INotificationHandler<DomainEventNotification<AppDeletedEvent>>
{
    private readonly ILogger<AppDeletedEventHandler> _logger;

    public AppDeletedEventHandler(ILogger<AppDeletedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(DomainEventNotification<AppDeletedEvent> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;

        _logger.LogInformation("Hippo Domain Event: {DomainEvent}", domainEvent.GetType().Name);

        return Task.CompletedTask;
    }
}