using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Hippo.Tasks;

namespace Hippo.Tests.Fakes
{
    public class FakeTaskQueue<T> : ITaskQueue<T>
    {
        private Queue<T> _impl;
        
        public Task<T> Dequeue(CancellationToken cancellationToken)
        {
            return Task.FromResult(_impl.Dequeue());
        }

        public Task Enqueue(T value, CancellationToken cancellationToken)
        {
            _impl.Enqueue(value);
            return Task.CompletedTask;
        }
    }
}