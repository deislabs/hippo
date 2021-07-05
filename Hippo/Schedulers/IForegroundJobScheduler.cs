using System.Collections.Generic;
using Hippo.Models;
using Hippo.Proxies;

namespace Hippo.Schedulers
{
    public interface IForegroundJobScheduler : IJobScheduler, IReverseProxy
    {
        void OnSchedulerStart(IEnumerable<Application> applications);

    }
}
