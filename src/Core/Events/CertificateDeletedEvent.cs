namespace Hippo.Core.Events;

public class CertificateDeletedEvent : DomainEvent
{
    public CertificateDeletedEvent(Certificate certificate)
    {
        Certificate = certificate;
    }

    public Certificate Certificate { get; }
}
