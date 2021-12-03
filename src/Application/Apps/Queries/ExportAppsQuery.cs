using AutoMapper;
using AutoMapper.QueryableExtensions;
using Hippo.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hippo.Application.Apps.Queries;

public class ExportAppsQuery : IRequest<ExportAppsVm>
{
}

public class ExportAppsQueryHandler : IRequestHandler<ExportAppsQuery, ExportAppsVm>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IJsonFileBuilder _fileBuilder;

    public ExportAppsQueryHandler(IApplicationDbContext context, IMapper mapper, IJsonFileBuilder fileBuilder)
    {
        _context = context;
        _mapper = mapper;
        _fileBuilder = fileBuilder;
    }

    public async Task<ExportAppsVm> Handle(ExportAppsQuery request, CancellationToken cancellationToken)
    {
        var records = await _context.Apps
            .ProjectTo<AppRecord>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        var vm = new ExportAppsVm(
                "apps.json",
                "application/json",
                _fileBuilder.BuildAppsFile(records));
        return vm;
    }
}
