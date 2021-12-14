namespace Hippo.Core.Events;

public class ChannelDeletedEvent : DomainEvent
{
    public ChannelDeletedEvent(Channel channel)
    {
        Channel = channel;
    }

    public Channel Channel { get; }
}
