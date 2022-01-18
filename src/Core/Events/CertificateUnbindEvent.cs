namespace Hippo.Core.Events;

public class CertificateUnbindEvent : DomainEvent
{
    public CertificateUnbindEvent(Certificate certificate, Channel channel)
    {
        Certificate = certificate;
        Channel = channel;
    }

    public Certificate Certificate { get; }

    public Channel Channel { get; }
}
