using System.ComponentModel.DataAnnotations;

namespace Hippo.Application.Channels.Queries;

public class JobStatus
{
    [Required]
    public Guid ChannelId { get; set; }

    [Required]
    public string Status { get; set; } = string.Empty;
}
