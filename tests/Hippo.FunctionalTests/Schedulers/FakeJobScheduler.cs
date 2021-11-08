using System.Collections.Generic;
using Hippo.Core.Models;
using Hippo.Infrastructure.Schedulers;

namespace Hippo.FunctionalTests.Schedulers
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

        public void Stop(Channel c)
        {
            // no-op
        }
    }
}
