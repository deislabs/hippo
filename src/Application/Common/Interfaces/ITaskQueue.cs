namespace Hippo.Application.Common.Interfaces;

public interface ITaskQueue<T>
{
    Task Enqueue(T value, CancellationToken cancellationToken);

    Task<T> Dequeue(CancellationToken cancellationToken);

    (bool, T?) TryRead();
}
