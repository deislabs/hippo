namespace Hippo.Core.Events;

public class DeletedEvent<T> : DomainEvent
{
    public DeletedEvent(T entity)
    {
        Entity = entity;
    }

    public T Entity { get; }
}
