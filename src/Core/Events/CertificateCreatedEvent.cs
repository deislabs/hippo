namespace Hippo.Core.Events;

public class CertificateCreatedEvent : DomainEvent
{
    public CertificateCreatedEvent(Certificate certificate)
    {
        Certificate = certificate;
    }

    public Certificate Certificate { get; }
}
