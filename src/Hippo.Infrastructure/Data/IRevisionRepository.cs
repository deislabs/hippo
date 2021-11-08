using Hippo.Core.Models;

namespace Hippo.Infrastructure.Data
{
    public interface IRevisionRepository
    {
        Revision GetRevisionByNumber(Application owner, string revisionNumber);
    }
}
