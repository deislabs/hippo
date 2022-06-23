using Hippo.Application.Common.Interfaces;
using Hippo.Core.Models;
using MediatR;

namespace Hippo.Application.Revisions.Queries;

public class GetStoragesQuery : SearchFilter, IRequest<Page<string>>
{
}

public class ExportStoragesQueryHandler : IRequestHandler<GetStoragesQuery, Page<string>>
{
    private readonly IBindleService _bindleService;

    public ExportStoragesQueryHandler(IBindleService bindleService)
    {
        _bindleService = bindleService;
    }

    public async Task<Page<string>> Handle(GetStoragesQuery request, CancellationToken cancellationToken)
    {
        var storages = (await _bindleService
            .QueryAvailableStorages(request.SearchText, (ulong)request.Offset, request.PageSize))
            .ScalarSortBy(request.IsSortedAscending)
            .ToList();

        var totalCount = (await _bindleService
            .QueryAvailableStorages(request.SearchText, null, null))
            .Count();

        return new Page<string>
        {
            Items = storages,
            PageIndex = request.PageIndex,
            PageSize = request.PageSize,
            TotalItems = totalCount
        };
    }
}
