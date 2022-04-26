using Deislabs.Bindle;
using Hippo.Application.Common.Interfaces;
using Hippo.Application.Revisions.Commands;
using Hippo.Core.Entities;
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

    public async Task<List<string>> GetBindleRevisionNumbers(string bindleId)
    {
        return (await _client.QueryInvoices(bindleId))
            .Where(i => i.Bindle.Name == bindleId) // this should be replaced with a direct API call when available
            .Select(i => i.Bindle.Version)
            .ToList();
    }
}
