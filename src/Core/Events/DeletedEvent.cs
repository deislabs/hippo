namespace Hippo.Core.Events;

public class DeletedEvent<T> : BaseEvent
{
    public DeletedEvent(T entity)
    {
        Entity = entity;
    }

    public T Entity { get; }
}
