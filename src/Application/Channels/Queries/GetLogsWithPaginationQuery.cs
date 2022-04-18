using Hippo.Application.Common.Exceptions;
using Hippo.Application.Common.Interfaces;
using Hippo.Application.Common.Models;
using Hippo.Core.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Hippo.Application.Channels.Queries;

public class GetLogsWithPaginationQuery : IRequest<PaginatedList<string>>
{
    public Guid ChannelId { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 1000;
}

public class GetLogsWithPaginationQueryHandler : IRequestHandler<GetLogsWithPaginationQuery, PaginatedList<string>>
{
    private readonly IApplicationDbContext _context;
    private readonly IJobFactory _jobFactory;
    private readonly ILogger<GetLogsWithPaginationQueryHandler> _logger;

    public GetLogsWithPaginationQueryHandler(IApplicationDbContext context, IJobFactory jobFactory, ILogger<GetLogsWithPaginationQueryHandler> logger)
    {
        _context = context;
        _jobFactory = jobFactory;
        _logger = logger;
    }

    public async Task<PaginatedList<string>> Handle(GetLogsWithPaginationQuery request, CancellationToken cancellationToken)
    {
        var channel =  await _context.Channels
            .Where(x => x.Id == request.ChannelId)
            .SingleOrDefaultAsync(cancellationToken);
        _ = channel ?? throw new NotFoundException(nameof(Channel), request.ChannelId);

        if (channel.ActiveRevision is not null)
        {
            _logger.LogInformation($"{channel.App.Name}: Fetching logs for channel {channel.Name} at revision {channel.ActiveRevision.RevisionNumber}");
            // return PaginatedList<string>.CreateAsync(_jobFactory.Logs(channel.Id), request.PageNumber, request.PageSize);
        }

        throw new NotImplementedException();
    }
}
