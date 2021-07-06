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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1716:Identifiers should not match keywords", Justification = "Don't care about VB")]
        public abstract void Stop(Channel c);

    }
}
