using Hippo.Application.Common.Interfaces;
using Hippo.Core.Models;
using MediatR;

namespace Hippo.Application.Revisions.Queries;

public class GetStoragesQuery : SearchFilter, IRequest<Page<string>>
{
}

public class ExportStoragesQueryHandler : IRequestHandler<GetStoragesQuery, Page<string>>
{
    private readonly IStorageService _storageService;

    public ExportStoragesQueryHandler(IStorageService storageService)
    {
        _storageService = storageService;
    }

    public async Task<Page<string>> Handle(GetStoragesQuery request, CancellationToken cancellationToken)
    {
        var storages = (await _storageService
            .QueryAvailableStorages(request.SearchText, (ulong)request.Offset, request.PageSize))
            .ScalarSortBy(request.IsSortedAscending)
            .ToList();

        var totalCount = (await _storageService
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
