using Hippo.Application.Apps.Queries;
using Hippo.Application.Channels.Queries;
using Hippo.Core.Entities;

namespace Hippo.Application.Apps.Extensions;

public static class AppExtensions
{
    public static Revision? GetLatestRevision(this App app)
    {
        return app.Revisions
            .OrderByDescending(r => r.RevisionNumber)
            .FirstOrDefault();
    }

    public static AppItem ToAppItem(this App app)
    {
        return new AppItem
        {
            Id = app.Id,
            Name = app.Name,
            StorageId = app.StorageId,
            Description = app.GetLatestRevision()?.Description,
            Channels = app.Channels.ToChannelSummaryDtoList(),
        };
    }

    public static AppSummaryDto ToAppSummaryDto(this App app)
    {
        return new AppSummaryDto
        {
            Id = app.Id,
            Name = app.Name,
            Channels = app.Channels.ToChannelSummaryDtoList(),
            StorageId = app.StorageId
        };
    }

    public static IOrderedEnumerable<AppItem> Sort(this List<AppItem> appItems, string sortBy, bool isSortedAscending)
    {
        IOrderedEnumerable<AppItem> orderedAppItems;
        var sortPropInfo = typeof(AppItem).GetProperty(sortBy);

        if (sortPropInfo == null)
        {
            throw new Exception("Sort field does it exist");
        }

        if (isSortedAscending)
        {
            orderedAppItems = appItems.OrderBy(app => sortPropInfo.GetValue(app, null));
        }
        else
        {
            orderedAppItems = appItems.OrderByDescending(app => sortPropInfo.GetValue(app, null));
        }

        return orderedAppItems;
    }
}
