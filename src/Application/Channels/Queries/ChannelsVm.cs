namespace Hippo.Application.Channels.Queries;

public class ChannelsVm
{
    public IList<ChannelDto> Channels { get; set; } = new List<ChannelDto>();
}
