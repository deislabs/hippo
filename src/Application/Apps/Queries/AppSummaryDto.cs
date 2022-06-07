using System.ComponentModel.DataAnnotations;

namespace Hippo.Application.Apps.Queries;

public class AppSummaryDto
{
    public AppSummaryDto()
    {
        Channels = new List<AppChannelSummary>();
    }

    [Required]
    public Guid Id { get; set; }

    [Required]
    public string Name { get; set; } = "";

    [Required]
    public IList<AppChannelSummary> Channels { get; set; }
}
