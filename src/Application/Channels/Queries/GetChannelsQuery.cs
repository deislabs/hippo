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

    public GetChannelsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ChannelsVm> Handle(GetChannelsQuery request, CancellationToken cancellationToken)
    {
        return new ChannelsVm
        {
            Channels = await _context.Channels
                .Include(c => c.App)
                .Select(c => c.ToChannelDto())
                .OrderBy(c => c.Name)
                .ToListAsync(cancellationToken)
        };
    }
}
