using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Hippo.Core.Interfaces;

namespace Hippo.FunctionalTests.Fakes
{
    public class FakeTaskQueue<T> : ITaskQueue<T>
    {
        private readonly Queue<T> _impl = new();

        public Task<T> Dequeue(CancellationToken cancellationToken)
        {
            return Task.FromResult(_impl.Dequeue());
        }

        public Task Enqueue(T value, CancellationToken cancellationToken)
        {
            _impl.Enqueue(value);
            return Task.CompletedTask;
        }

        public (bool, T) TryRead()
        {
            var item = _impl.Dequeue();
            return (item is not null, item);
        }
    }
}
