using AutoMapper;
using AutoMapper.QueryableExtensions;
using Hippo.Application.Common.Exceptions;
using Hippo.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hippo.Application.Revisions.Queries;

public class GetActiveRevisionDetailsQuery : IRequest<RevisionDetailsVm?>
{
    public Guid ChannelId { get; set; }
}

public class GetActiveRevisionDetailsQueryHandler : IRequestHandler<GetActiveRevisionDetailsQuery, RevisionDetailsVm?>
{
    private readonly IApplicationDbContext _context;

    private readonly IMapper _mapper;

    public GetActiveRevisionDetailsQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<RevisionDetailsVm?> Handle(GetActiveRevisionDetailsQuery request, CancellationToken cancellationToken)
    {
        var channel = _context.Channels
            .Where(c => c.Id == request.ChannelId)
            .Include(c => c.ActiveRevision)
            .FirstOrDefault();
        if (channel is null)
        {
            throw new NotFoundException($"Channel Id: {request.ChannelId}");
        }

        if (channel.ActiveRevision is null)
        {
            return null;
        }

        return new RevisionDetailsVm
        {
            Id = channel.ActiveRevision.Id,
            RevisionNumber = channel.ActiveRevision.RevisionNumber,
            Description = channel.ActiveRevision.Description,
            Type = channel.ActiveRevision.Type,
            Base = channel.ActiveRevision.Base,
            Components = await GetRevisionComponents(channel.ActiveRevision.Id, cancellationToken),
        };
    }

    private async Task<List<RevisionComponentDto>> GetRevisionComponents(Guid revisionId, CancellationToken cancellationToken)
    {
        return await _context.RevisionComponents
            .Where(c => c.RevisionId == revisionId)
            .ProjectTo<RevisionComponentDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
    }
}
