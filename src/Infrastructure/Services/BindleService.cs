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

    public async Task<IEnumerable<string?>> GetBindleRevisionNumbers(string bindleId)
    {
        return (await _client.QueryInvoices(bindleId))
            .Invoices
            .Where(i => i.Bindle.Name == bindleId)
            .Select(i => i.Bindle.Version)
            .ToList();
    }
}
