
using Hippo.Application.Apps.Extensions;
using Hippo.Application.Apps.Queries;
using Hippo.Core.Entities;

namespace Hippo.Application.Channels.Queries;

public static class ChannelExtensions
{
    public static AppChannelSummary ToApplicationChannelSummaryDto(this Channel channel)
    {
        return new AppChannelSummary
        {
            Id = channel.Id,
            Name = channel.Name,
            ActiveRevisionNumber = channel.ActiveRevision?.RevisionNumber,
        };
    }

    public static IList<AppChannelSummary> ToChannelSummaryDtoList(this IList<Channel> channels)
    {
        return channels.OrderBy(c => c.Created).Select(x => x.ToApplicationChannelSummaryDto()).ToList();
    }
}
