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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1716:Identifiers should not match keywords", Justification = "Don't care about VB")]
        void Stop(Channel c);
    }
}
