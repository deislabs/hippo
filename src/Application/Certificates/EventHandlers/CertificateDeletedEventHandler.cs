using Hippo.Core.Entities;
using Hippo.Core.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hippo.Application.Certificates.EventHandlers;

public class CertificateDeletedEventHandler : INotificationHandler<DeletedEvent<Certificate>>
{
    private readonly ILogger<CertificateDeletedEventHandler> _logger;

    public CertificateDeletedEventHandler(ILogger<CertificateDeletedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(DeletedEvent<Certificate> notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Hippo Domain Event: {notification.GetType().Name}");

        return Task.CompletedTask;
    }
}
