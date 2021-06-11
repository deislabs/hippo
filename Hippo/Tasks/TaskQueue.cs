using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Hippo.Tasks
{
    public interface ITaskQueue<T>
    {
        ValueTask Enqueue(T value, CancellationToken cancellationToken);
        ValueTask<T> Dequeue(CancellationToken cancellationToken);
    }

    public class TaskQueue<T>: ITaskQueue<T>
    {
        private readonly Channel<T> _queue;

        public TaskQueue(int capacity)
        {
            var options = new BoundedChannelOptions(capacity) { FullMode = BoundedChannelFullMode.Wait };
            _queue = Channel.CreateBounded<T>(options);
        }

        public async ValueTask Enqueue(T value, CancellationToken cancellationToken)
        {
            await _queue.Writer.WriteAsync(value, cancellationToken);
        }

        public async ValueTask<T> Dequeue(CancellationToken cancellationToken)
        {
            return await _queue.Reader.ReadAsync(cancellationToken);
        }
    }
}