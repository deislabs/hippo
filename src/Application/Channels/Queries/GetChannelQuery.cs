using System.ComponentModel.DataAnnotations;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Hippo.Application.Apps.Extensions;
using Hippo.Application.Common.Exceptions;
using Hippo.Application.Common.Interfaces;
using Hippo.Core.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hippo.Application.Channels.Queries;

public class GetChannelQuery : IRequest<ChannelItem>
{
    [Required]
    public Guid Id { get; set; }
}

public class GetChannelQueryHandler : IRequestHandler<GetChannelQuery, ChannelItem>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetChannelQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ChannelItem> Handle(GetChannelQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.Channels
            .Where(a => a.Id == request.Id)
            .ProjectTo<ChannelItem>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken);

        if (entity is null)
        {
            throw new NotFoundException(nameof(Channel), request.Id);
        }

        entity.AppSummary = await _context.Apps
            .Where(a => a.Id == entity.AppId)
            .Include(a => a.Channels)
            .Select(a => a.ToAppSummaryDto())
            .FirstOrDefaultAsync(cancellationToken);

        return entity;
    }
}
