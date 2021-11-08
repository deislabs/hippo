using System.Threading;
using System.Threading.Tasks;

namespace Hippo.Core.Interfaces
{
    public interface ITaskQueue<T>
    {
        Task Enqueue(T value, CancellationToken cancellationToken);
        Task<T> Dequeue(CancellationToken cancellationToken);
        (bool, T) TryRead();
    }
}
