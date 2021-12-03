using Hippo.Application.Common.Models;
using Hippo.Core.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hippo.Application.Domains.EventHandlers;

public class DomainCreatedEventHandler : INotificationHandler<DomainEventNotification<DomainCreatedEvent>>
{
    private readonly ILogger<DomainCreatedEventHandler> _logger;

    public DomainCreatedEventHandler(ILogger<DomainCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(DomainEventNotification<DomainCreatedEvent> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;

        _logger.LogInformation("Hippo Domain Event: {DomainEvent}", domainEvent.GetType().Name);

        return Task.CompletedTask;
    }
}
