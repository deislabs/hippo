using AutoMapper;
using AutoMapper.QueryableExtensions;
using Hippo.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hippo.Application.Revisions.Queries;

public class ExportStoragesQuery : IRequest<ExportStoragesQueryVm>
{
    public ExportStoragesQuery(string queryString, ulong? offset, int? limit)
    {
        QueryString = queryString;
        Limit = limit;
        Offset = offset;
    }

    public string QueryString { get; set; }
    public int? Limit { get; set; }
    public ulong? Offset { get; set; }
}

public class ExportStoragesQueryHandler : IRequestHandler<ExportStoragesQuery, ExportStoragesQueryVm>
{
    private readonly IBindleService _bindleService;

    public ExportStoragesQueryHandler(IBindleService bindleService)
    {
        _bindleService = bindleService;
    }

    public async Task<ExportStoragesQueryVm> Handle(ExportStoragesQuery request, CancellationToken cancellationToken)
    {
        var storages = await _bindleService.QueryAvailableStorages(request.QueryString, request.Offset, request.Limit);
        var vm = new ExportStoragesQueryVm(storages);
        return vm;
    }
}
