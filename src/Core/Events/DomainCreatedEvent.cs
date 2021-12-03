namespace Hippo.Core.Events;

public class DomainCreatedEvent : DomainEvent
{
    public DomainCreatedEvent(Domain domain)
    {
        Domain = domain;
    }

    public Domain Domain { get; }
}
