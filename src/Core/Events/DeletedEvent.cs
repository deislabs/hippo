namespace Hippo.Core.Events;

public class DeletedEvent<E> : DomainEvent
{
    public DeletedEvent(E entity)
    {
        Entity = entity;
    }

    public E Entity { get; }
}
