using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hippo.Core.Models;

namespace Hippo.Infrastructure.Data
{
    public interface IEventLogRepository
    {
        Task LoginSucceeded(EventOrigin source, string userName);
        Task LoginFailed(EventOrigin source, string userName, string reason);

        Task ChannelCreated(EventOrigin source, Channel channel);
        Task ChannelEdited(EventOrigin source, Channel channel);
        Task ChannelRevisionChanged(EventOrigin source, Channel channel, string oldRevision, string reason);
        Task ChannelDeleted(EventOrigin source, Guid channelId, Application application, string channelName);

        IEnumerable<EventLogEntry> GetRecentByApplication(Application application, int maxCount);
    }
}
