using System.ComponentModel.DataAnnotations;

namespace Hippo.Application.Channels.Queries;

public class GetChannelLogsVm
{
    public GetChannelLogsVm(List<string> logs)
    {
        Logs = logs;
    }

    [Required]
    public List<string> Logs { get; set; }
}
