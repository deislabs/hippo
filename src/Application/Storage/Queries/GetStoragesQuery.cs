using AutoMapper;
using AutoMapper.QueryableExtensions;
using Hippo.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hippo.Application.Revisions.Queries;

public class GetStoragesQuery : IRequest<GetStoragesQueryVm>
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

public class ExportStoragesQueryHandler : IRequestHandler<GetStoragesQuery, GetStoragesQueryVm>
{
    private readonly IBindleService _bindleService;

    public ExportStoragesQueryHandler(IBindleService bindleService)
    {
        _bindleService = bindleService;
    }

    public async Task<GetStoragesQueryVm> Handle(GetStoragesQuery request, CancellationToken cancellationToken)
    {
        var storages = await _bindleService.QueryAvailableStorages(request.QueryString, request.Offset, request.Limit);
        var vm = new GetStoragesQueryVm(storages);
        return vm;
    }
}
