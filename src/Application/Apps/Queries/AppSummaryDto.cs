using System.ComponentModel.DataAnnotations;
using Hippo.Application.Channels.Queries;

namespace Hippo.Application.Apps.Queries;

public class AppSummaryDto
{
    public AppSummaryDto()
    {
        Channels = new List<ApplicationChannelSummary>();
    }

    [Required]
    public Guid Id { get; set; }

    [Required]
    public string Name { get; set; } = "";

    [Required]
    public IList<ApplicationChannelSummary> Channels { get; set; }
}
