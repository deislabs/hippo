using Hippo.Core.Entities;
using Hippo.Core.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hippo.Application.Apps.EventHandlers;

public class AppCreatedEventHandler : INotificationHandler<CreatedEvent<App>>
{
    private readonly ILogger<AppCreatedEventHandler> _logger;

    public AppCreatedEventHandler(ILogger<AppCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(CreatedEvent<App> notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Hippo Domain Event: {DomainEvent}", notification.GetType().Name);

        return Task.CompletedTask;
    }
}
