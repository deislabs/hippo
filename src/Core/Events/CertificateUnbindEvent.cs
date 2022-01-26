namespace Hippo.Core.Events;

public class CertificateUnbindEvent : DomainEvent
{
    public CertificateUnbindEvent(Channel channel)
    {
        Channel = channel;
    }

    public Channel Channel { get; }
}
