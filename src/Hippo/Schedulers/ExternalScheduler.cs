using System.Collections.Generic;
using Hippo.Messages;
using Hippo.Models;
using Hippo.Proxies;
using Microsoft.Extensions.Logging;

namespace Hippo.Schedulers
{
    public abstract class ExternalScheduler : IJobScheduler
    {
        public virtual void OnSchedulerStart(IEnumerable<Application> applications)
        {
        }

        public abstract void Start(Channel c);

        public abstract void Stop(Channel c);

    }
}
