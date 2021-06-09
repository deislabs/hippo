using System.Collections.Generic;
using Hippo.Models;

namespace Hippo.Schedulers
{
    public interface IJobScheduler
    {
        void OnSchedulerStart(IEnumerable<Application> applications);

        /// <summary>
        /// Start the current release.
        /// </summary>
        void Start(Channel c);

        /// <summary>
        /// Gracefully shut down the current release. This prevents the channel
        /// from receiving requests.
        /// </summary>
        void Stop(Channel c);
    }
}
