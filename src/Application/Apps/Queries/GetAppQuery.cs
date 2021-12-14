using AutoMapper;
using AutoMapper.QueryableExtensions;
using Hippo.Application.Common.Exceptions;
using Hippo.Application.Common.Interfaces;
using Hippo.Core.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hippo.Application.Apps.Queries;

public class GetAppQuery : IRequest<AppDto>
{
    public Guid Id { get; set; }
}

public class GetAppQueryHandler : IRequestHandler<GetAppQuery, AppDto>
{
    private readonly IApplicationDbContext _context;

    private readonly IMapper _mapper;

    public GetAppQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<AppDto> Handle(GetAppQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.Apps
            .Where(a => a.Id == request.Id)
            .ProjectTo<AppDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException(nameof(App), request.Id);
        }

        return entity;
    }
}
