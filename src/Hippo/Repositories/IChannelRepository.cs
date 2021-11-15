using System;
using System.Threading.Tasks;
using Hippo.Models;

namespace Hippo.Repositories;

public interface IChannelRepository
{
    Channel GetChannelByName(Application owner, string name);
    Channel GetChannelById(Guid id);
    Task AddNew(Channel channel);
    void DeleteChannelById(Guid id);
}
