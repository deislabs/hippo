using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hippo.Models;

namespace Hippo.Repositories
{
    public class DbEventLogRepository : IEventLogRepository
    {
        private readonly DataContext _context;
        private readonly ICurrentUser _user;

        public DbEventLogRepository(DataContext context, ICurrentUser user)
        {
            _context = context;
            _user = user;
        }

        public async Task LoginSucceeded(EventOrigin source, string userName)
        {
            var entry = new EventLogEntry
            {
                EventKind = EventKind.AccountLogin,
                EventSource = source,
                Timestamp = DateTime.UtcNow,
                UserName = userName,
                Description = "login succeeded",
            };
            await _context.EventLogEntries.AddAsync(entry);
        }

        public async Task LoginFailed(EventOrigin source, string userName, string reason)
        {
            var entry = new EventLogEntry
            {
                EventKind = EventKind.AccountLoginFailed,
                EventSource = source,
                Timestamp = DateTime.UtcNow,
                UserName = userName,
                Description = reason,
            };
            await _context.EventLogEntries.AddAsync(entry);
        }

        public async Task ChannelRevisionChanged(EventOrigin source, Channel channel, string oldRevision, string reason)
        {
            var entry = new EventLogEntry
            {
                EventKind = EventKind.ChannelRevisionChanged,
                EventSource = source,
                Timestamp = DateTime.UtcNow,
                ApplicationId = channel.Application.Id,
                ChannelId = channel.Id,
                UserName = _user.Name(),
                Description = $"changed from {oldRevision} to {channel.ActiveRevision?.RevisionNumber ?? "(none)"} because {reason}",
            };
            await _context.EventLogEntries.AddAsync(entry);
        }

        public IEnumerable<EventLogEntry> GetRecentByApplication(Application application, int maxCount)
        {
            return _context.EventLogEntries
                .Where(e => e.ApplicationId == application.Id)
                .OrderByDescending(e => e.Timestamp)
                .Take(maxCount);
        }
    }
}
