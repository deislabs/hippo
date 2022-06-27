using Hippo.Application.Common.Interfaces;
using MediatR;

namespace Hippo.Application.Revisions.Queries;

public class GetStoragesQuery : IRequest<StorageList>
{
    public GetStoragesQuery(string queryString, ulong? offset, int? limit)
    {
        QueryString = queryString;
        Limit = limit;
        Offset = offset;
    }

    public string QueryString { get; set; }
    public int? Limit { get; set; }
    public ulong? Offset { get; set; }
}

public class ExportStoragesQueryHandler : IRequestHandler<GetStoragesQuery, StorageList>
{
    private readonly IStorageService _storageService;

    public ExportStoragesQueryHandler(IStorageService storageService)
    {
        _storageService = storageService;
    }

    public async Task<StorageList> Handle(GetStoragesQuery request, CancellationToken cancellationToken)
    {
        var storages = await _storageService.QueryAvailableStorages(request.QueryString, request.Offset, request.Limit);
        var vm = new StorageList(storages.ToList());
        return vm;
    }
}
