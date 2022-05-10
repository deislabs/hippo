
using Hippo.Core.Entities;

namespace Hippo.Application.Channels.Queries;

public static class ChannelExtensions
{
    public static ChannelSummaryDto ToChannelSummaryDto(this Channel channel)
    {
        return new ChannelSummaryDto
        {
            Id = channel.Id,
            Name = channel.Name,
            ActiveRevisionNumber = channel.ActiveRevision?.RevisionNumber,
        };
    }

    public static IList<ChannelSummaryDto> ToChannelSummaryDtoList(this IList<Channel> channels)
    {
        return channels.Select(x => x.ToChannelSummaryDto()).ToList();
    }
}
