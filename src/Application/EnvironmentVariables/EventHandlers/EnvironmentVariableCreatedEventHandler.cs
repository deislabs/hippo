using Hippo.Application.Common.Models;
using Hippo.Core.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hippo.Application.EnvironmentVariables.EventHandlers;

public class EnvironmentVariableCreatedEventHandler : INotificationHandler<DomainEventNotification<EnvironmentVariableCreatedEvent>>
{
    private readonly ILogger<EnvironmentVariableCreatedEventHandler> _logger;

    public EnvironmentVariableCreatedEventHandler(ILogger<EnvironmentVariableCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(DomainEventNotification<EnvironmentVariableCreatedEvent> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;

        _logger.LogInformation("Hippo Domain Event: {DomainEvent}", domainEvent.GetType().Name);

        return Task.CompletedTask;
    }
}
