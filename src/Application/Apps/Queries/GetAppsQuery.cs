using Hippo.Application.Apps.Extensions;
using Hippo.Application.Common.Interfaces;
using Hippo.Core.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hippo.Application.Apps.Queries;

public class GetAppsQuery : SearchFilter, IRequest<Page<AppItem>>
{
}

public class GetAppsQueryHandler : IRequestHandler<GetAppsQuery, Page<AppItem>>
{
    private readonly IApplicationDbContext _context;

    public GetAppsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Page<AppItem>> Handle(GetAppsQuery request, CancellationToken cancellationToken)
    {
        var apps = _context.Apps
                .Include(a => a.Channels)
                .Include(a => a.Revisions);

        var appItems = (await apps
            .Where(app => app.Name.Contains(request.SearchText))
            .Select(a => a.ToAppItem())
            .ToListAsync())
            .SortBy(request.SortBy, request.IsSortedAscending)
            .Skip(request.Offset)
            .Take(request.PageSize)
            .ToList();

        return new Page<AppItem>
        {
            Items = appItems,
            PageIndex = request.PageIndex,
            PageSize = request.PageSize,
            TotalItems = apps.Count()
        };
    }
}
