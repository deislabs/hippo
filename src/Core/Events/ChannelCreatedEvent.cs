namespace Hippo.Core.Events;

public class ChannelCreatedEvent : DomainEvent
{
    public ChannelCreatedEvent(Channel channel)
    {
        Channel = channel;
    }

    public Channel Channel { get; }
}
