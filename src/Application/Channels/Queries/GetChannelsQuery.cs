using AutoMapper;
using AutoMapper.QueryableExtensions;
using Hippo.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hippo.Application.Channels.Queries;

public class GetChannelsQuery : IRequest<ChannelsVm>
{
}

public class GetChannelsQueryHandler : IRequestHandler<GetChannelsQuery, ChannelsVm>
{
    private readonly IApplicationDbContext _context;

    private readonly IMapper _mapper;

    public GetChannelsQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ChannelsVm> Handle(GetChannelsQuery request, CancellationToken cancellationToken)
    {
        return new ChannelsVm
        {
            Channels = await _context.Channels
                .ProjectTo<ChannelDto>(_mapper.ConfigurationProvider)
                .OrderBy(c => c.Name)
                .ToListAsync(cancellationToken)
        };
    }
}
