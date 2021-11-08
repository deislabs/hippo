using System;
using System.Threading.Tasks;
using Hippo.Core.Models;

namespace Hippo.Infrastructure.Data
{
    public interface IChannelRepository
    {
        Channel GetChannelByName(Application owner, string name);
        Channel GetChannelById(Guid id);
        Task AddNew(Channel channel);
        void DeleteChannelById(Guid id);
    }
}
