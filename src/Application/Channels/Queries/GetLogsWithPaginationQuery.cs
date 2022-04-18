using Hippo.Application.Common.Models;
using MediatR;

namespace Hippo.Application.Channels.Queries;

public class GetLogsWithPaginationQuery : IRequest<PaginatedList<string>>
{
    public Guid ChannelId { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 1000;
}

public class GetLogsWithPaginationQueryHandler : IRequestHandler<GetLogsWithPaginationQuery, PaginatedList<string>>
{
    public Task<PaginatedList<string>> Handle(GetLogsWithPaginationQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}