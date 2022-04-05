namespace Hippo.Core.Events;

public class CreatedEvent<E> : DomainEvent
{
    public CreatedEvent(E entity)
    {
        Entity = entity;
    }

    public E Entity { get; }
}
