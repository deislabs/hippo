using System.ComponentModel.DataAnnotations;

namespace Hippo.Application.Apps.Queries;

public class AppDto
{
    public AppDto()
    {
        Channels = new List<AppChannelSummary>();
    }

    [Required]
    public Guid Id { get; set; }

    [Required]
    public string Name { get; set; } = "";

    [Required]
    public string StorageId { get; set; } = "";

    public string? Description { get; set; } = "";

    [Required]
    public IList<AppChannelSummary> Channels { get; set; }
}
