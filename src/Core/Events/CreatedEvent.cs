namespace Hippo.Core.Events;

public class CreatedEvent<T> : BaseEvent
{
    public CreatedEvent(T entity)
    {
        Entity = entity;
    }

    public T Entity { get; }
}
