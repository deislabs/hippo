namespace Hippo.Core.Events;

public class RevisionCreatedEvent : DomainEvent
{
    public RevisionCreatedEvent(Revision revision)
    {
        Revision = revision;
    }

    public Revision Revision { get; }
}
