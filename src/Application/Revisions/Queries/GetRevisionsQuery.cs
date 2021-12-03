using AutoMapper;
using AutoMapper.QueryableExtensions;
using Hippo.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hippo.Application.Revisions.Queries;

public class GetRevisionsQuery : IRequest<RevisionsVm>
{
}

public class GetRevisionsQueryHandler : IRequestHandler<GetRevisionsQuery, RevisionsVm>
{
    private readonly IApplicationDbContext _context;

    private readonly IMapper _mapper;

    public GetRevisionsQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<RevisionsVm> Handle(GetRevisionsQuery request, CancellationToken cancellationToken)
    {
        return new RevisionsVm
        {
            Revisions = await _context.Revisions
                .ProjectTo<RevisionDto>(_mapper.ConfigurationProvider)
                .OrderBy(r => r.RevisionNumber)
                .ToListAsync(cancellationToken)
        };
    }
}
