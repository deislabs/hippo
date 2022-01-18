namespace Hippo.Core.Events;

public class CertificateBindEvent : DomainEvent
{
    public CertificateBindEvent(Certificate certificate, Channel channel)
    {
        Certificate = certificate;
        Channel = channel;
    }

    public Certificate Certificate { get; }

    public Channel Channel { get; }
}
