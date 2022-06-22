using System.ComponentModel.DataAnnotations;

namespace Hippo.Application.Apps.Queries;

public class AppChannelListItem
{
    [Required]
    public Guid Id { get; set; }

    [Required]
    public string Name { get; set; } = "";

    public string? ActiveRevisionNumber { get; set; }
}
