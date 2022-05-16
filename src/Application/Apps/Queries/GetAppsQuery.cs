using AutoMapper;
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
                .Select(a => new AppDto
                {
                    Id = a.Id,
                    Name = a.Name,
                    StorageId = a.StorageId,
                    Channels = a.Channels.ToChannelSummaryDtoList(),
                })
                .OrderBy(a => a.Name)
                .ToListAsync(cancellationToken)
        };
    }
}
