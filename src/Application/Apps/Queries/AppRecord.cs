using Hippo.Application.Channels.Queries;
using Hippo.Application.Common.Mappings;
using Hippo.Application.Revisions.Queries;
using Hippo.Core.Entities;

namespace Hippo.Application.Apps.Queries;

public class AppRecord : IMapFrom<App>
{
    public AppRecord()
    {
        Channels = new List<ChannelRecord>();
        Revisions = new List<RevisionRecord>();
    }

    public string? Name { get; set; }

    public string? StorageId { get; set; }

    public IList<ChannelRecord> Channels { get; set; }

    public IList<RevisionRecord> Revisions { get; set; }
}
