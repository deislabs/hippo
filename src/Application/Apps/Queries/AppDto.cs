using System.ComponentModel.DataAnnotations;
using Hippo.Application.Channels.Queries;

namespace Hippo.Application.Apps.Queries;

public class AppDto
{
    public AppDto()
    {
        Channels = new List<ApplicationChannelSummary>();
    }

    [Required]
    public Guid Id { get; set; }

    [Required]
    public string Name { get; set; } = "";

    [Required]
    public string StorageId { get; set; } = "";

    [Required]
    public string? Description { get; set; } = "";

    [Required]
    public IList<ApplicationChannelSummary> Channels { get; set; }
}
