using AutoMapper;
using AutoMapper.QueryableExtensions;
using Hippo.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hippo.Application.Certificates.Queries;

public class GetCertificatesQuery : IRequest<CertificatesVm>
{
}

public class GetCertificatesQueryHandler : IRequestHandler<GetCertificatesQuery, CertificatesVm>
{
    private readonly IApplicationDbContext _context;

    private readonly IMapper _mapper;

    public GetCertificatesQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<CertificatesVm> Handle(GetCertificatesQuery request, CancellationToken cancellationToken)
    {
        return new CertificatesVm
        {
            Certificates = await _context.Certificates
                .ProjectTo<CertificateDto>(_mapper.ConfigurationProvider)
                .OrderBy(c => c.Name)
                .ToListAsync(cancellationToken)
        };
    }
}
