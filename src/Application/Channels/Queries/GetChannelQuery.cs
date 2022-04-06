using System.ComponentModel.DataAnnotations;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Hippo.Application.Common.Exceptions;
using Hippo.Application.Common.Interfaces;
using Hippo.Core.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hippo.Application.Channels.Queries;

public class GetChannelQuery : IRequest<ChannelDto>
{
    [Required]
    public Guid Id { get; set; }
}

public class GetChannelQueryHandler : IRequestHandler<GetChannelQuery, ChannelDto>
{
    private readonly IApplicationDbContext _context;

    private readonly IMapper _mapper;

    public GetChannelQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ChannelDto> Handle(GetChannelQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.Channels
            .Where(a => a.Id == request.Id)
            .ProjectTo<ChannelDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken);

        if (entity is null)
        {
            throw new NotFoundException(nameof(Channel), request.Id);
        }

        return entity;
    }
}
