using System.ComponentModel.DataAnnotations;
using Hippo.Application.Apps.Queries;
using Hippo.Application.Certificates.Queries;
using Hippo.Application.Common.Attributes;
using Hippo.Application.Common.Mappings;
using Hippo.Application.EnvironmentVariables.Queries;
using Hippo.Application.Revisions.Queries;
using Hippo.Core.Entities;
using Hippo.Core.Enums;

namespace Hippo.Application.Channels.Queries;

public class ChannelItem : IMapFrom<Channel>
{
    public ChannelItem()
    {
        EnvironmentVariables = new List<EnvironmentVariableDto>();
    }

    [Required]
    public Guid Id { get; set; }

    [Required]
    public Guid AppId { get; set; }

    [Required]
    public string Name { get; set; } = "";

    [Required]
    public string Domain { get; set; } = "";

    [Required]
    public ChannelRevisionSelectionStrategy RevisionSelectionStrategy { get; set; }

    public RevisionDto? ActiveRevision { get; set; }

    public string? RangeRule { get; set; }

    public CertificateDto? Certificate { get; set; }

    [Required]
    public IList<EnvironmentVariableDto> EnvironmentVariables { get; set; }

    [NoMap]
    public AppSummaryDto? AppSummary { get; set; }
}
