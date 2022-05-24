using AutoMapper;
using Hippo.Application.Apps.Extensions;
using Hippo.Application.Channels.Queries;
using Hippo.Application.Common.Interfaces;
using Hippo.Core.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hippo.Application.Apps.Queries;

public class GetAppsQuery : IRequest<AppsVm>
{
}

public class GetAppsQueryHandler : IRequestHandler<GetAppsQuery, AppsVm>
{
    private readonly IApplicationDbContext _context;

    public GetAppsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<AppsVm> Handle(GetAppsQuery request, CancellationToken cancellationToken)
    {
        return new AppsVm
        {
            Apps = await _context.Apps
                .OrderBy(a => a.Name)
                .Include(a => a.Channels)
                .Include(a => a.Revisions)
                .Select(a => a.ToAppDto())
                .ToListAsync(cancellationToken)
        };
    }
}
