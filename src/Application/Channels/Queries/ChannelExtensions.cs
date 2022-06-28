using Hippo.Application.Apps.Queries;
using Hippo.Core.Entities;

namespace Hippo.Application.Channels.Queries;

public static class ChannelExtensions
{
    public static AppChannelListItem ToApplicationChannelSummaryDto(this Channel channel)
    {
        return new AppChannelListItem
        {
            Id = channel.Id,
            Name = channel.Name,
            ActiveRevisionNumber = channel.ActiveRevision?.RevisionNumber,
        };
    }

    public static IList<AppChannelListItem> ToChannelSummaryDtoList(this IList<Channel> channels)
    {
        return channels.OrderBy(c => c.Created).Select(x => x.ToApplicationChannelSummaryDto()).ToList();
    }
}
