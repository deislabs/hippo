using System.ComponentModel.DataAnnotations;

namespace Hippo.Application.Apps.Queries;

public class AppChannelSummary
{
    [Required]
    public Guid Id { get; set; }

    [Required]
    public string Name { get; set; } = "";

    public string? ActiveRevisionNumber { get; set; }
}
