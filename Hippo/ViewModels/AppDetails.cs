using System.Collections.Generic;
using Hippo.Models;

namespace Hippo.ViewModels
{
    public class AppDetails
    {
        public Application Application { get; set; }
        public IReadOnlyCollection<Channel> Channels { get; set; }
        public IReadOnlyCollection<Revision> Revisions { get; set; }
        public IReadOnlyCollection<EventLogEntry> RecentActivity { get; set; }
    }
}
