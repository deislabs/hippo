using AutoMapper;
using AutoMapper.QueryableExtensions;
using Hippo.Application.Common.Interfaces;
using Hippo.Core.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hippo.Application.Certificates.Queries;

public class GetCertificatesQuery : SearchFilter, IRequest<Page<CertificateItem>>
{
}

public class GetCertificatesQueryHandler : IRequestHandler<GetCertificatesQuery, Page<CertificateItem>>
{
    private readonly IApplicationDbContext _context;

    private readonly IMapper _mapper;

    public GetCertificatesQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Page<CertificateItem>> Handle(GetCertificatesQuery request, CancellationToken cancellationToken)
    {
        var certificates = await _context.Certificates
                .ProjectTo<CertificateItem>(_mapper.ConfigurationProvider)
                .Where(c => request.SearchText == null || c.Name.Contains(request.SearchText))
                .ToListAsync(cancellationToken);

        var certificateItems = certificates
            .SortBy(request.SortBy, request.IsSortedAscending)
            .Skip(request.Offset)
            .Take(request.PageSize)
            .ToList();

        return new Page<CertificateItem>
        {
            Items = certificateItems,
            PageIndex = request.PageIndex,
            PageSize = request.PageSize,
            TotalItems = certificates.Count()
        };
    }
}
