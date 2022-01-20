using AutoMapper;
using AutoMapper.QueryableExtensions;
using Hippo.Application.Common.Exceptions;
using Hippo.Application.Common.Interfaces;
using Hippo.Core.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hippo.Application.Certificates.Queries;

public class GetCertificateQuery : IRequest<CertificateDto>
{
    public Guid Id { get; set; }
}

public class GetCertificateQueryHandler : IRequestHandler<GetCertificateQuery, CertificateDto>
{
    private readonly IApplicationDbContext _context;

    private readonly IMapper _mapper;

    public GetCertificateQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<CertificateDto> Handle(GetCertificateQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.Certificates
            .Where(a => a.Id == request.Id)
            .ProjectTo<CertificateDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException(nameof(Certificate), request.Id);
        }

        return entity;
    }
}
