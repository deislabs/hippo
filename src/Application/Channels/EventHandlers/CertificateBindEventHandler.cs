using Hippo.Application.Common.Models;
using Hippo.Core.Events;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Hippo.Application.Channels.EventHandlers;

public class CertificateBindEventHandler : INotificationHandler<DomainEventNotification<CertificateBindEvent>>
{
    private readonly ILogger<CertificateBindEventHandler> _logger;

    private readonly IConfiguration _configuration;

    public CertificateBindEventHandler(ILogger<CertificateBindEventHandler> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public Task Handle(DomainEventNotification<CertificateBindEvent> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;

        _logger.LogInformation("Hippo Domain Event: {DomainEvent}", domainEvent.GetType().Name);

        // Add cert to kestrel config; kestrel will automatically reload
        // https://docs.microsoft.com/en-us/dotnet/core/compatibility/aspnet-core/5.0/kestrel-configuration-changes-at-run-time-detected-by-default
        //
        // NOTE: It is safe to assume that a domain has been added thanks to CreateChannelCommandValidator.
        //
        // TODO: Do we need to handle cases when the domain name changes? Perhaps we should handle that with a new event.
        //       That being said, it is likely the certificate will need to be replaced... So this may not be an issue.
        var certificate = notification.DomainEvent.Certificate;
        var domain = notification.DomainEvent.Channel.Domain!;
        var sniOptions = new SniOptions(new SniOptions.CertificateOptions(certificate.PublicKey!, certificate.PrivateKey!, Path.Combine(System.IO.Directory.GetCurrentDirectory(), domain)));

        _configuration.GetSection($"{SniOptions.Position}:{notification.DomainEvent.Channel.Domain!}").Bind(sniOptions);

        return Task.CompletedTask;
    }
}

public class SniOptions
{
    public const string Position = "Kestrel:Endpoints:Https:Sni";

    public string Protocols = "Http1AndHttp2";

    public List<string> SslProtocols = new List<string>() { "Tls11", "Tls12", "Tls13" };

    public CertificateOptions Certificate;

    public SniOptions(CertificateOptions certificate)
    {
        Certificate = certificate;
    }

    public class CertificateOptions
    {
        public string Path;

        public string KeyPath;

        public string? Password;

        public CertificateOptions(string publicKey, string privateKey, string rootPath)
        {
            // save to disk so kestrel can serve it
            Path = System.IO.Path.Combine(rootPath, "pub.pem");
            KeyPath = System.IO.Path.Combine(rootPath, "key.crt");

            using (StreamWriter pubKeyData = new StreamWriter(Path, true))
            {
                pubKeyData.Write(publicKey);
            }

            using (StreamWriter pubKeyData = new StreamWriter(KeyPath, true))
            {
                pubKeyData.Write(privateKey);
            }
        }
    }
}
