using System.ComponentModel.DataAnnotations;

namespace Hippo.Application.Channels.Queries;

public class ApplicationChannelSummary
{
    [Required]
    public Guid Id { get; set; }

    [Required]
    public string Name { get; set; } = "";

    public string? ActiveRevisionNumber { get; set; }
}
