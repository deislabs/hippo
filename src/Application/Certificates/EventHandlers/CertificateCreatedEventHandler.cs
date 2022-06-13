using Hippo.Core.Entities;
using Hippo.Core.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hippo.Application.Certificates.EventHandlers;

public class CertificateCreatedEventHandler : INotificationHandler<CreatedEvent<Certificate>>
{
    private readonly ILogger<CertificateCreatedEventHandler> _logger;

    public CertificateCreatedEventHandler(ILogger<CertificateCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(CreatedEvent<Certificate> notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Hippo Domain Event: {notification.GetType().Name}");

        return Task.CompletedTask;
    }
}
