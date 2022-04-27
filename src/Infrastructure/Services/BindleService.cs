using Deislabs.Bindle;
using Hippo.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Hippo.Infrastructure.Services;

public class BindleService : IBindleService
{
    private BindleClient _client { get; set; }

    public BindleService(IConfiguration configuration)
    {
        var bindleUrl = configuration.GetValue<string>("Bindle:Url", "http://127.0.0.1:8080/v1");
        _client = new BindleClient(bindleUrl);
    }

    public async Task<List<string>> QueryAvailableStorages(string query, ulong? offset, int? limit)
    {
        var invoices = await _client.QueryInvoices(query, offset, limit);
        return invoices.Select(i => i.Bindle.Name).Distinct().ToList();
    }
}
