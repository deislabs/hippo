using AutoMapper;
using AutoMapper.QueryableExtensions;
using Hippo.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hippo.Application.Domains.Queries;

public class GetDomainsQuery : IRequest<DomainsVm>
{
}

public class GetDomainsQueryHandler : IRequestHandler<GetDomainsQuery, DomainsVm>
{
    private readonly IApplicationDbContext _context;

    private readonly IMapper _mapper;

    public GetDomainsQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<DomainsVm> Handle(GetDomainsQuery request, CancellationToken cancellationToken)
    {
        return new DomainsVm
        {
            Domains = await _context.Domains
                .ProjectTo<DomainDto>(_mapper.ConfigurationProvider)
                .OrderBy(d => d.Name)
                .ToListAsync(cancellationToken)
        };
    }
}
