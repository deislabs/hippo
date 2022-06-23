using System.ComponentModel.DataAnnotations;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Hippo.Application.Common.Exceptions;
using Hippo.Application.Common.Interfaces;
using Hippo.Core.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hippo.Application.Certificates.Queries;

public class GetCertificateQuery : IRequest<CertificateItem>
{
    [Required]
    public Guid Id { get; set; }
}

public class GetCertificateQueryHandler : IRequestHandler<GetCertificateQuery, CertificateItem>
{
    private readonly IApplicationDbContext _context;

    private readonly IMapper _mapper;

    public GetCertificateQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<CertificateItem> Handle(GetCertificateQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.Certificates
            .Where(a => a.Id == request.Id)
            .ProjectTo<CertificateItem>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken);

        if (entity is null)
        {
            throw new NotFoundException(nameof(Certificate), request.Id);
        }

        return entity;
    }
}
