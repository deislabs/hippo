using System.Collections.Generic;
using System.Linq;
using Hippo.Models;

namespace Hippo.ViewModels
{
    public class AppDetails
    {
        public Application Application { get; set; }
        public IReadOnlyCollection<Channel> Channels { get; set; }
        public IReadOnlyCollection<Revision> Revisions { get; set; }
        public IReadOnlyCollection<EventLogEntry> RecentActivity { get; set; }

        public string ChannelNameFor(EventLogEntry evt)
        {
            if (evt.ChannelId.HasValue)
            {
                return Channels.FirstOrDefault(c => c.Id == evt.ChannelId)?.Name ?? evt.ChannelId?.ToString();
            }
            return string.Empty;
        }
    }
}
