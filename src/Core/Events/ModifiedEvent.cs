namespace Hippo.Core.Events;

public class ModifiedEvent<T> : BaseEvent
{
    public ModifiedEvent(T entity)
    {
        Entity = entity;
    }

    public T Entity { get; }
}
