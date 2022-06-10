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

    public static AppDto ToAppDto(this App app)
    {
        return new AppDto
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
}
