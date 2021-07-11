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

        Task ChannelCreated(EventOrigin source, Channel channel);
        Task ChannelEdited(EventOrigin source, Channel channel);
        Task ChannelRevisionChanged(EventOrigin source, Channel channel, string oldRevision, string reason);

        IEnumerable<EventLogEntry> GetRecentByApplication(Application application, int maxCount);
    }
}
