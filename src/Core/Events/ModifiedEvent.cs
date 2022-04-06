namespace Hippo.Core.Events;

public class ModifiedEvent<T> : DomainEvent
{
    public ModifiedEvent(T entity)
    {
        Entity = entity;
    }

    public T Entity { get; }
}
