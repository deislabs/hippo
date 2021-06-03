using Hippo.Models;

namespace Hippo.Repositories
{
    public interface IReleaseRepository
    {
        Release GetReleaseByRevision(Application owner, string revision);
    }
}