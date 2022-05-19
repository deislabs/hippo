using System.ComponentModel.DataAnnotations;
using Hippo.Application.Apps.Queries;
using Hippo.Application.Common.Mappings;
using Hippo.Application.EnvironmentVariables.Queries;
using Hippo.Core.Entities;
using Hippo.Core.Enums;

namespace Hippo.Application.Channels.Queries;

public class ChannelDto
{
    [Required]
    public Guid Id { get; set; }

    [Required]
    public string Name { get; set; } = "";

    [Required]
    public string? Domain { get; set; } = "";

    [Required]
    public AppSummaryDto? AppSummary { get; set; }
}
