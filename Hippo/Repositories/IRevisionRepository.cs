using Hippo.Models;

namespace Hippo.Repositories
{
    public interface IRevisionRepository
    {
        Revision GetRevisionByNumber(Application owner, string revisionNumber);
    }
}
