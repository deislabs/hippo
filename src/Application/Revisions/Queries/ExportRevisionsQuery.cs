using AutoMapper;
using AutoMapper.QueryableExtensions;
using Hippo.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hippo.Application.Revisions.Queries;

public class ExportRevisionsQuery : IRequest<ExportRevisionsVm>
{
}

public class ExportRevisionsQueryHandler : IRequestHandler<ExportRevisionsQuery, ExportRevisionsVm>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IJsonFileBuilder _fileBuilder;

    public ExportRevisionsQueryHandler(IApplicationDbContext context, IMapper mapper, IJsonFileBuilder fileBuilder)
    {
        _context = context;
        _mapper = mapper;
        _fileBuilder = fileBuilder;
    }

    public async Task<ExportRevisionsVm> Handle(ExportRevisionsQuery request, CancellationToken cancellationToken)
    {
        var records = await _context.Revisions
            .ProjectTo<RevisionRecord>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        var vm = new ExportRevisionsVm(
                "revisions.json",
                "application/json",
                _fileBuilder.BuildRevisionsFile(records));
        return vm;
    }
}
