using Deislabs.Bindle;
using Hippo.Application.Common.Exceptions;
using Hippo.Application.Common.Interfaces;
using Hippo.Application.Common.Interfaces.BindleService;
using Microsoft.Extensions.Configuration;
using Tomlyn;

namespace Hippo.Infrastructure.Services;

public class BindleService : IBindleService
{
    private BindleClient _client { get; set; }

    public BindleService(IConfiguration configuration)
    {
        var bindleUrl = configuration.GetValue("Bindle:Url", "http://127.0.0.1:8080/v1");
        _client = new BindleClient(bindleUrl);
    }

    public async Task<RevisionDetails> GetRevisionDetails(string revisionId)
    {
        var invoice = await _client.GetInvoice(revisionId);
        if (invoice is null || invoice.Bindle is null)
        {
            throw new NotFoundException($"Revision Id: {revisionId}");
        }

        return new RevisionDetails
        {
            Name = invoice.Bindle.Name,
            Version = invoice.Bindle.Version,
            Description = invoice.Bindle.Description,
            Authors = invoice.Bindle.Authors,
            SpinToml = await GetSpinTomlParcel(invoice, revisionId),
        };
    }

    private async Task<RevisionSpinToml?> GetSpinTomlParcel(Invoice invoice, string revisionId)
    {
        var spinTomlFileType = "application/vnd.fermyon.spin+toml";
        var spinTomlData = invoice.Parcels
            .FirstOrDefault(p => p.Label?.MediaType == spinTomlFileType);
        var parcelId = spinTomlData?.Label?.Sha256;

        if (spinTomlData is null || parcelId is null)
        {
            return null;
        }
        
        var parcelHttpResponse = await _client.GetParcel(revisionId, parcelId);
        return await ParseSpinTomlParcel(parcelHttpResponse);
    }

    private static async Task<RevisionSpinToml> ParseSpinTomlParcel(HttpContent parcelResponse)
    {
        var content = await parcelResponse.ReadAsStringAsync();
        var parsedContent = Toml.Parse(content);
        if (parsedContent.HasErrors)
        {
            throw new ArgumentException($"Error parsing Toml content");
        }

        var tomlOptions = new TomlModelOptions
        {
            ConvertPropertyName = name => TomlNamingHelper.PascalToCamelCase(name)
        };

        return parsedContent.ToModel<RevisionSpinToml>(tomlOptions);
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
    
    public async Task<IEnumerable<string>> QueryAvailableStorages(string query, ulong? offset, int? limit)
    {
        var matches = await _client.QueryInvoices(query, offset, limit);
        if (matches.Total == 0)
            return new List<string>();
        return matches.Invoices.Where(i => i.Bindle != null && !string.IsNullOrEmpty(i.Bindle.Name))
            .Select(i => new string(i.Bindle?.Name)).Distinct();
    }
}
