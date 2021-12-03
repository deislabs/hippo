using AutoMapper;
using AutoMapper.QueryableExtensions;
using Hippo.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hippo.Application.Domains.Queries;

public class ExportDomainsQuery : IRequest<ExportDomainsVm>
{
}

public class ExportDomainsQueryHandler : IRequestHandler<ExportDomainsQuery, ExportDomainsVm>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IJsonFileBuilder _fileBuilder;

    public ExportDomainsQueryHandler(IApplicationDbContext context, IMapper mapper, IJsonFileBuilder fileBuilder)
    {
        _context = context;
        _mapper = mapper;
        _fileBuilder = fileBuilder;
    }

    public async Task<ExportDomainsVm> Handle(ExportDomainsQuery request, CancellationToken cancellationToken)
    {
        var records = await _context.Domains
            .ProjectTo<DomainRecord>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        var vm = new ExportDomainsVm(
                "domains.json",
                "application/json",
                _fileBuilder.BuildDomainsFile(records));
        return vm;
    }
}
