namespace Hippo.Core.Events;

public class RevisionDeletedEvent : DomainEvent
{
    public RevisionDeletedEvent(Revision revision)
    {
        Revision = revision;
    }

    public Revision Revision { get; }
}
