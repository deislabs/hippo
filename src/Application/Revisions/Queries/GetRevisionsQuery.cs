using AutoMapper;
using AutoMapper.QueryableExtensions;
using Hippo.Application.Common.Interfaces;
using Hippo.Core.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hippo.Application.Revisions.Queries;

public class GetRevisionsQuery : SearchFilter, IRequest<Page<RevisionItem>>
{
}

public class GetRevisionsQueryHandler : IRequestHandler<GetRevisionsQuery, Page<RevisionItem>>
{
    private readonly IApplicationDbContext _context;

    private readonly IMapper _mapper;

    public GetRevisionsQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Page<RevisionItem>> Handle(GetRevisionsQuery request, CancellationToken cancellationToken)
    {
        var revisions = await _context.Revisions
                .ProjectTo<RevisionItem>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

        var revisionsPage = revisions
            .Skip(request.Offset)
            .Take(request.PageSize)
            .ToList();

        return new Page<RevisionItem>
        {
            Items = revisionsPage,
            PageIndex = request.PageIndex,
            PageSize = request.PageSize,
            TotalItems = revisions.Count
        };
    }
}
