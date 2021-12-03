namespace Hippo.Core.Events;

public class ActiveRevisionChangedEvent : DomainEvent
{
    public ActiveRevisionChangedEvent(Channel channel, Revision changedTo)
    {
        Channel = channel;
        ChangedFrom = channel.ActiveRevision;
        ChangedTo = changedTo;
    }

    public Channel Channel { get; }

    public Revision? ChangedFrom { get; }

    public Revision ChangedTo { get; }
}
