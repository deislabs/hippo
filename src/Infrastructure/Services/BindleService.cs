using Deislabs.Bindle;
using Hippo.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Hippo.Infrastructure.Services;

public class BindleService : IBindleService
{
    private BindleClient _client { get; set; }

    public BindleService(IConfiguration configuration)
    {
        var bindleUrl = configuration.GetValue("Bindle:Url", "http://127.0.0.1:8080/v1");
        _client = new BindleClient(bindleUrl);
    }

    public async Task<IEnumerable<string>> GetBindleRevisionNumbers(string bindleId)
    {
        var matches = await _client.QueryInvoices(bindleId);
        if (matches.Total == 0)
            return new List<string>();

        return matches.Invoices
            .Where(i => i.Bindle is not null)
            .Where(i => i.Bindle?.Name == bindleId)
            .Select(i => new string(i.Bindle?.Version))
            .ToList();
    }
}
