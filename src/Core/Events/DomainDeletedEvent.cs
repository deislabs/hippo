namespace Hippo.Core.Events;

public class DomainDeletedEvent : DomainEvent
{
    public DomainDeletedEvent(Domain domain)
    {
        Domain = domain;
    }

    public Domain Domain { get; }
}
