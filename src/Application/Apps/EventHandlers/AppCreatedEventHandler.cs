using Hippo.Application.Common.Models;
using Hippo.Core.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hippo.Application.Apps.EventHandlers;

public class AppCreatedEventHandler : INotificationHandler<DomainEventNotification<AppCreatedEvent>>
{
    private readonly ILogger<AppCreatedEventHandler> _logger;

    public AppCreatedEventHandler(ILogger<AppCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(DomainEventNotification<AppCreatedEvent> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;

        _logger.LogInformation("Hippo Domain Event: {DomainEvent}", domainEvent.GetType().Name);

        return Task.CompletedTask;
    }
}
