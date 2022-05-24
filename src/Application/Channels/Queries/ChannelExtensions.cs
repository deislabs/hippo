
using Hippo.Application.Apps.Extensions;
using Hippo.Core.Entities;

namespace Hippo.Application.Channels.Queries;

public static class ChannelExtensions
{
    public static ApplicationChannelSummary ToApplicationChannelSummaryDto(this Channel channel)
    {
        return new ApplicationChannelSummary
        {
            Id = channel.Id,
            Name = channel.Name,
            ActiveRevisionNumber = channel.ActiveRevision?.RevisionNumber,
        };
    }

    public static IList<ApplicationChannelSummary> ToChannelSummaryDtoList(this IList<Channel> channels)
    {
        return channels.OrderBy(c => c.Created).Select(x => x.ToApplicationChannelSummaryDto()).ToList();
    }

    public static ChannelSummaryDto ToChannelSummaryDto(this Channel channel)
    {
        return new ChannelSummaryDto
        {
            Id = channel.Id,
            Name = channel.Name,
            Domain = channel.Domain,
            AppSummary = channel.App.ToAppSummaryDto(),
        };
    }
}
