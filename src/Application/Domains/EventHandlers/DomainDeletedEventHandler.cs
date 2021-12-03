using Hippo.Application.Common.Models;
using Hippo.Core.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hippo.Application.Domains.EventHandlers;

public class DomainDeletedEventHandler : INotificationHandler<DomainEventNotification<DomainDeletedEvent>>
{
    private readonly ILogger<DomainDeletedEventHandler> _logger;

    public DomainDeletedEventHandler(ILogger<DomainDeletedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(DomainEventNotification<DomainDeletedEvent> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;

        _logger.LogInformation("Hippo Domain Event: {DomainEvent}", domainEvent.GetType().Name);

        return Task.CompletedTask;
    }
}
