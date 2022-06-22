using System.ComponentModel.DataAnnotations;

namespace Hippo.Application.Apps.Queries;

public class AppItem
{
    public AppItem()
    {
        Channels = new List<AppChannelListItem>();
    }

    [Required]
    public Guid Id { get; set; }

    [Required]
    public string Name { get; set; } = "";

    [Required]
    public string StorageId { get; set; } = "";

    public string? Description { get; set; } = "";

    [Required]
    public IList<AppChannelListItem> Channels { get; set; }
}
