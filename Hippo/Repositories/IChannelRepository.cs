using System.Threading.Tasks;
using Hippo.Models;

namespace Hippo.Repositories
{
    public interface IChannelRepository
    {
        Channel GetChannelByName(Application owner, string name);
    }
}