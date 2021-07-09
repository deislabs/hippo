using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hippo.Models;

namespace Hippo.Repositories
{
    public interface IEventLogRepository
    {
        Task LoginSucceeded(EventOrigin source, string userName);
        Task LoginFailed(EventOrigin source, string userName, string reason);
        Task ChannelRevisionChanged(EventOrigin source, Channel channel, string oldRevision, string reason);
    }
}
