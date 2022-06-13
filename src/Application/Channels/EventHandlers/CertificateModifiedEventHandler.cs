using Hippo.Core.Entities;
using Hippo.Core.Events;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Hippo.Application.Channels.EventHandlers;

public class CertificateModifiedEventHandler : INotificationHandler<ModifiedEvent<Channel>>
{
    private readonly ILogger<CertificateModifiedEventHandler> _logger;

    private readonly IConfiguration _configuration;

    public CertificateModifiedEventHandler(ILogger<CertificateModifiedEventHandler> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public Task Handle(ModifiedEvent<Channel> notification, CancellationToken cancellationToken)
    {
        Channel channel = notification.Entity;

        _logger.LogInformation($"Hippo Domain Event: {notification.GetType().Name}");

        if (channel.Certificate is null || channel.Domain is null)
        {
            // remove certificate from kestrel config
            // TODO: we should probably delete the certificate file here
            _configuration.GetSection($"{SniOptions.Position}:{channel.Domain}").Bind(null);
            return Task.CompletedTask;
        }

        // Add cert to kestrel config; kestrel will automatically reload
        // https://docs.microsoft.com/en-us/dotnet/core/compatibility/aspnet-core/5.0/kestrel-configuration-changes-at-run-time-detected-by-default
        //
        // NOTE: It is safe to assume that a domain has been added thanks to CreateChannelCommandValidator.
        //
        // TODO: Do we need to handle cases when the domain name changes? Perhaps we should handle that with a new event.
        //       That being said, it is likely the certificate will need to be replaced... So this may not be an issue.
        var sniOptions = new SniOptions(new SniOptions.CertificateOptions(channel.Certificate.PublicKey!, channel.Certificate.PrivateKey!, Path.Combine(System.IO.Directory.GetCurrentDirectory(), channel.Domain)));

        _configuration.GetSection($"{SniOptions.Position}:{channel.Domain}").Bind(sniOptions);

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
