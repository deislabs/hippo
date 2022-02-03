using Hippo.Application.Common.Mappings;
using Hippo.Application.EnvironmentVariables.Queries;
using Hippo.Application.Revisions.Queries;
using Hippo.Core.Entities;
using Hippo.Core.Enums;

namespace Hippo.Application.Channels.Queries;

public class ChannelRecord : IMapFrom<Channel>
{
    public ChannelRecord()
    {
        EnvironmentVariables = new List<EnvironmentVariableRecord>();
    }

    public string Name { get; set; } = "";

    public string Domain { get; set; } = "";

    public ChannelRevisionSelectionStrategy RevisionSelectionStrategy { get; set; }

    public RevisionRecord? ActiveRevision { get; set; }

    public string? RangeRule { get; set; }

    public IList<EnvironmentVariableRecord> EnvironmentVariables { get; set; }
}
