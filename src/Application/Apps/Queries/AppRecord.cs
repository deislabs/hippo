using System.ComponentModel.DataAnnotations;
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

    [Required]
    public string Name { get; set; } = "";

    [Required]
    public string StorageId { get; set; } = "";

    [Required]
    public IList<ChannelRecord> Channels { get; set; }

    [Required]
    public IList<RevisionRecord> Revisions { get; set; }
}
