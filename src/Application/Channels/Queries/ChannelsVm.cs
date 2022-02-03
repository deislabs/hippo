using System.ComponentModel.DataAnnotations;

namespace Hippo.Application.Channels.Queries;

public class ChannelsVm
{
    [Required]
    public IList<ChannelDto> Channels { get; set; } = new List<ChannelDto>();
}
