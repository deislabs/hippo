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

    public GetChannelQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ChannelDto> Handle(GetChannelQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.Channels
            .Where(a => a.Id == request.Id)
            .Include(a => a.App)
            .Select(a => a.ToChannelDto())
            .FirstOrDefaultAsync(cancellationToken);

        if (entity is null)
        {
            throw new NotFoundException(nameof(Channel), request.Id);
        }

        return entity;
    }
}
