using AutoMapper;
using AutoMapper.QueryableExtensions;
using Hippo.Application.Common.Interfaces;
using Hippo.Core.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hippo.Application.Channels.Queries;

public class GetChannelsQuery : SearchFilter, IRequest<Page<ChannelItem>>
{
}

public class GetChannelsQueryHandler : IRequestHandler<GetChannelsQuery, Page<ChannelItem>>
{
    private readonly IApplicationDbContext _context;

    private readonly IMapper _mapper;

    public GetChannelsQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Page<ChannelItem>> Handle(GetChannelsQuery request, CancellationToken cancellationToken)
    {
        var channels = await _context.Channels
                .ProjectTo<ChannelItem>(_mapper.ConfigurationProvider)
                .OrderBy(c => c.Name)
                .ToListAsync(cancellationToken);

        var channelsPage = channels
            .Skip(request.Offset)
            .Take(request.PageSize)
            .ToList();

        return new Page<ChannelItem>
        {
            Items = channelsPage,
            PageIndex = request.PageIndex,
            PageSize = request.PageSize,
            TotalItems = channels.Count
        };
    }
}
