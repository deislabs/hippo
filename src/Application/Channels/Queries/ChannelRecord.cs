using System.ComponentModel.DataAnnotations;
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

    [Required]
    public string Name { get; set; } = "";

    [Required]
    public string Domain { get; set; } = "";

    [Required]
    public ChannelRevisionSelectionStrategy RevisionSelectionStrategy { get; set; }

    public RevisionRecord? ActiveRevision { get; set; }

    public string? RangeRule { get; set; }

    [Required]
    public IList<EnvironmentVariableRecord> EnvironmentVariables { get; set; }
}
