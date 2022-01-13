using Hippo.Application.Channels.Queries;
using Hippo.Application.Common.Mappings;
using Hippo.Application.Revisions.Queries;
using Hippo.Core.Entities;

namespace Hippo.Application.Apps.Queries;

public class AppDto : IMapFrom<App>
{
    public AppDto()
    {
        Channels = new List<ChannelDto>();
        Revisions = new List<RevisionDto>();
    }

    public Guid Id { get; set; }

    public string? Name { get; set; }

    public IList<ChannelDto> Channels { get; set; }

    public IList<RevisionDto> Revisions { get; set; }
}
