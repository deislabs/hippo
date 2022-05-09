using Hippo.Application.Common.Exceptions;
using Hippo.Application.Common.Interfaces;
using MediatR;

namespace Hippo.Application.Revisions.Models;

public class GetActiveRevisionDetailsQuery : IRequest<RevisionDetails?>
{
    public Guid ChannelId { get; set; }
}

public class GetActiveRevisionDetailsQueryHandler : IRequestHandler<GetActiveRevisionDetailsQuery, RevisionDetails?>
{
    private readonly IApplicationDbContext _context;
    private readonly IBindleService _bindleService;

    public GetActiveRevisionDetailsQueryHandler(IApplicationDbContext context, IBindleService bindleService)
    {
        _context = context;
        _bindleService = bindleService;
    }

    public async Task<RevisionDetails?> Handle(GetActiveRevisionDetailsQuery request, CancellationToken cancellationToken)
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
