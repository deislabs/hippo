namespace Hippo.Core.Events;

public class CertificateBindEvent : DomainEvent
{
    public CertificateBindEvent(Channel channel)
    {
        Channel = channel;
    }

    public Channel Channel { get; }
}
