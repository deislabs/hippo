namespace Hippo.Core.Events;

public class ActiveRevisionChangedEvent : DomainEvent
{
    public ActiveRevisionChangedEvent(Channel channel)
    {
        Channel = channel;
    }

    public Channel Channel { get; }
}
