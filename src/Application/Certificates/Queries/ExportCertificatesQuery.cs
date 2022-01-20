using AutoMapper;
using AutoMapper.QueryableExtensions;
using Hippo.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hippo.Application.Certificates.Queries;

public class ExportCertificatesQuery : IRequest<ExportCertificatesVm>
{
}

public class ExportCertificatesQueryHandler : IRequestHandler<ExportCertificatesQuery, ExportCertificatesVm>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IJsonFileBuilder _fileBuilder;

    public ExportCertificatesQueryHandler(IApplicationDbContext context, IMapper mapper, IJsonFileBuilder fileBuilder)
    {
        _context = context;
        _mapper = mapper;
        _fileBuilder = fileBuilder;
    }

    public async Task<ExportCertificatesVm> Handle(ExportCertificatesQuery request, CancellationToken cancellationToken)
    {
        var records = await _context.Certificates
            .ProjectTo<CertificateRecord>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        var vm = new ExportCertificatesVm(
                "certificates.json",
                "application/json",
                _fileBuilder.BuildCertificatesFile(records));
        return vm;
    }
}
