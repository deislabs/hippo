using System.Collections.Generic;
using Hippo.Models;
using Hippo.Schedulers;

namespace Hippo.Tests.Schedulers
{
    public class FakeJobScheduler : IJobScheduler
    {
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
