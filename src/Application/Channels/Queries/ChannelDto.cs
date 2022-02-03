using Hippo.Application.Common.Mappings;
using Hippo.Application.EnvironmentVariables.Queries;
using Hippo.Core.Entities;
using Hippo.Core.Enums;

namespace Hippo.Application.Channels.Queries;

public class ChannelDto : IMapFrom<Channel>
{
    public ChannelDto()
    {
        EnvironmentVariables = new List<EnvironmentVariableDto>();
    }

    public Guid Id { get; set; }

    public Guid AppId { get; set; }

    public string Name { get; set; } = "";

    public string Domain { get; set; } = "";

    public ChannelRevisionSelectionStrategy RevisionSelectionStrategy { get; set; }

    public Revision? ActiveRevision { get; set; }

    public string? RangeRule { get; set; }

    public IList<EnvironmentVariableDto> EnvironmentVariables { get; set; }
}
