using Hippo.Application.Common.Models;
using Hippo.Core.Events;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Hippo.Application.Channels.EventHandlers;

public class CertificateUnbindEventHandler : INotificationHandler<DomainEventNotification<ChannelCreatedEvent>>
{
    private readonly ILogger<CertificateUnbindEventHandler> _logger;

    private readonly IConfiguration _configuration;

    public CertificateUnbindEventHandler(ILogger<CertificateUnbindEventHandler> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public Task Handle(DomainEventNotification<ChannelCreatedEvent> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;

        _logger.LogInformation("Hippo Domain Event: {DomainEvent}", domainEvent.GetType().Name);

        // remove certificate from kestrel config
        // TODO: we should probably delete the certificate file here
        _configuration.GetSection($"{SniOptions.Position}:{notification.DomainEvent.Channel.Domain}").Bind(null);

        return Task.CompletedTask;
    }
}
