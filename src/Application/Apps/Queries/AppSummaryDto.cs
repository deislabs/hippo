using System.ComponentModel.DataAnnotations;
using Hippo.Application.Channels.Queries;

namespace Hippo.Application.Apps.Queries;

public class AppSummaryDto
{
    public AppSummaryDto()
    {
        Channels = new List<ChannelSummaryDto>();
    }

    [Required]
    public Guid Id { get; set; }

    [Required]
    public string Name { get; set; } = "";

    [Required]
    public IList<ChannelSummaryDto> Channels { get; set; }
}
