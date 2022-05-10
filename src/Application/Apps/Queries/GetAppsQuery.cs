using AutoMapper;
using AutoMapper.QueryableExtensions;
using Hippo.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hippo.Application.Apps.Queries;

public class GetAppsQuery : IRequest<AppsVm>
{
}

public class GetAppsQueryHandler : IRequestHandler<GetAppsQuery, AppsVm>
{
    private readonly IApplicationDbContext _context;

    private readonly IMapper _mapper;

    public GetAppsQueryHandler(IApplicationDbContext context,
        IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<AppsVm> Handle(GetAppsQuery request, CancellationToken cancellationToken)
    {
        var apps = await _context.Apps
            .ProjectTo<AppDto>(_mapper.ConfigurationProvider)
            .OrderBy(a => a.Name)
            .ToListAsync(cancellationToken);

        return new AppsVm
        {
            Apps = apps
        };
    }
}
