using Hippo.Application.Jobs;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Hippo.Application.ChannelStatuses.Queries;

public class ChannelJobStatusItem
{
    [Required]
    public Guid ChannelId { get; set; }

    [Required]
    public JobStatus Status { get; set; }
}
