using System.Collections.Generic;
using Hippo.Models;
using Hippo.Schedulers;

namespace Hippo.Tests.Schedulers
{
    public class FakeJobScheduler : IJobScheduler
    {
        public void OnSchedulerStart(IEnumerable<Application> applications)
        {
            // no-op
        }

        public void Start(Channel c)
        {
            // no-op
        }

        public ChannelStatus Status(Channel c)
        {
            // no-op
            return new();
        }

        public void Stop(Channel c)
        {
            // no-op
        }
    }
}
