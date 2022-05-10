using System.ComponentModel.DataAnnotations;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Hippo.Application.Common.Exceptions;
using Hippo.Application.Common.Interfaces;
using Hippo.Application.Revisions.Queries;
using Hippo.Core.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hippo.Application.Channels.Queries;

public class GetChannelDetailsQuery : IRequest<ChannelDetailsDto>
{
    [Required]
    public Guid ChannelId { get; set; }
}

public class GetChannelDetailsQueryHandler : IRequestHandler<GetChannelDetailsQuery, ChannelDetailsDto>
{
    private readonly IApplicationDbContext _context;

    private readonly IMapper _mapper;

    public GetChannelDetailsQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ChannelDetailsDto> Handle(GetChannelDetailsQuery request, CancellationToken cancellationToken)
    {
        var channelDetails = await _context.Channels
            .Where(a => a.Id == request.ChannelId)
            .ProjectTo<ChannelDetailsDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken);

        if (channelDetails is null)
        {
            throw new NotFoundException(nameof(Channel), request.ChannelId);
        }

        return channelDetails;
    }
}
