using System.ComponentModel.DataAnnotations;

namespace Hippo.Application.Apps.Queries;

public class AppSummaryDto
{
    public AppSummaryDto()
    {
        Channels = new List<AppChannelListItem>();
    }

    [Required]
    public Guid Id { get; set; }

    [Required]
    public string Name { get; set; } = "";

    [Required]
    public string StorageId { get; set; } = "";

    [Required]
    public IList<AppChannelListItem> Channels { get; set; }
}
