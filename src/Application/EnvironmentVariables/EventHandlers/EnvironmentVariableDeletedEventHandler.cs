using Hippo.Application.Common.Models;
using Hippo.Core.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hippo.Application.EnvironmentVariables.EventHandlers;

public class EnvironmentVariableDeletedEventHandler : INotificationHandler<DomainEventNotification<EnvironmentVariableDeletedEvent>>
{
    private readonly ILogger<EnvironmentVariableDeletedEventHandler> _logger;

    public EnvironmentVariableDeletedEventHandler(ILogger<EnvironmentVariableDeletedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(DomainEventNotification<EnvironmentVariableDeletedEvent> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;

        _logger.LogInformation("Hippo Domain Event: {DomainEvent}", domainEvent.GetType().Name);

        return Task.CompletedTask;
    }
}
