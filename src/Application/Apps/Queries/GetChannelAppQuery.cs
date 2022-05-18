using System.ComponentModel.DataAnnotations;
using Hippo.Application.Apps.Extensions;
using Hippo.Application.Common.Exceptions;
using Hippo.Application.Common.Interfaces;
using Hippo.Core.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hippo.Application.Apps.Queries;

public class GetChannelAppQuery : IRequest<AppDto>
{
    [Required]
    public Guid ChannelId { get; set; }
}

public class GetChannelAppQueryHandler : IRequestHandler<GetChannelAppQuery, AppDto>
{
    private readonly IApplicationDbContext _context;

    public GetChannelAppQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<AppDto> Handle(GetChannelAppQuery request, CancellationToken cancellationToken)
    {
        var channel = await _context.Channels
            .Where(c => c.Id == request.ChannelId)
            .Include(c => c.App)
            .Include(c => c.App.Channels)
            .Include(c => c.App.Revisions)
            .FirstOrDefaultAsync(cancellationToken);

        if (channel is null || channel.App is null)
        {
            throw new NotFoundException(nameof(Channel), request.ChannelId);
        }

        var entity = channel.App.ToAppDto();
        entity.SelectedChannelUrl = channel.Domain;

        return entity;
    }
}
