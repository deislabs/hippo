using Hippo.Core.Entities;

namespace Hippo.Application.Common.Interfaces;

public interface IJobScheduler
{
    /// <summary>
    /// Schedule the current release.
    /// </summary>
    void Start(Channel c);

    /// <summary>
    /// Gracefully shut down the current release.
    /// </summary>
    void Stop(Channel c);
}
