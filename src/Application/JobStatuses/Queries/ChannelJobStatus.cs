using Hippo.Application.Jobs;
using System.ComponentModel.DataAnnotations;

namespace Hippo.Application.Channels.Queries;

public class ChannelJobStatus
{
    [Required]
    public Guid ChannelId { get; set; }

    [Required]
    public JobStatus Status { get; set; }
}
