using AutoMapper;
using AutoMapper.QueryableExtensions;
using Hippo.Application.Common.Exceptions;
using Hippo.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hippo.Application.Revisions.Queries;

public class GetActiveRevisionDetailsQuery : IRequest<RevisionDetailsDto?>
{
    public Guid ChannelId { get; set; }
}

public class GetActiveRevisionDetailsQueryHandler : IRequestHandler<GetActiveRevisionDetailsQuery, RevisionDetailsDto?>
{
    private readonly IApplicationDbContext _context;
    private readonly IBindleService _bindleService;

    public GetActiveRevisionDetailsQueryHandler(IApplicationDbContext context, IBindleService bindleService)
    {
        _context = context;
        _bindleService = bindleService;
    }

    public async Task<RevisionDetailsDto?> Handle(GetActiveRevisionDetailsQuery request, CancellationToken cancellationToken)
    {
        var channel = _context.Channels
            .Where(c => c.Id == request.ChannelId)
            .FirstOrDefault();
        if (channel is null)
        {
            throw new NotFoundException($"Channel Id: {request.ChannelId}");
        }

        if (channel.ActiveRevision is null)
        {
            return null;
        }

        var revisionId = $"{channel.App.StorageId}/{channel.ActiveRevision.RevisionNumber}";

        return await _bindleService.GetRevisionDetails(revisionId);
    }
}
