using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Hippo.Tasks
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1711:Identifiers should not have incorrect suffix", Justification = "Not applicable for this type")]
    public interface ITaskQueue<T>
    {
        Task Enqueue(T value, CancellationToken cancellationToken);
        Task<T> Dequeue(CancellationToken cancellationToken);
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1711:Identifiers should not have incorrect suffix", Justification = "Not applicable for this type")]
    public class TaskQueue<T> : ITaskQueue<T>
    {
        const int DEFAULT_BUFFER_CAPACITY = 1024;

        private readonly Channel<T> _queue;

        public TaskQueue() : this(DEFAULT_BUFFER_CAPACITY) { }

        public TaskQueue(int capacity)
        {
            var options = new BoundedChannelOptions(capacity) { FullMode = BoundedChannelFullMode.Wait };
            _queue = Channel.CreateBounded<T>(options);
        }

        public async Task Enqueue(T value, CancellationToken cancellationToken)
        {
            await _queue.Writer.WriteAsync(value, cancellationToken);
        }

        public async Task<T> Dequeue(CancellationToken cancellationToken)
        {
            return await _queue.Reader.ReadAsync(cancellationToken);
        }
    }
}
