using Deislabs.Bindle;
using Hippo.Application.Common.Exceptions;
using Hippo.Application.Common.Interfaces;
using Hippo.Application.Revisions.Queries;
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

    public async Task<RevisionDetailsDto> GetRevisionDetails(string revisionId)
    {
        var invoice = await _client.GetInvoice(revisionId);
        if (invoice is null)
        {
            throw new NotFoundException($"Revision Id: {revisionId}");
        }

        return new RevisionDetailsDto
        {
            Name = invoice.Bindle.Name,
            Version = invoice.Bindle.Version,
            Description = invoice.Bindle.Description,
            Authors = invoice.Bindle.Authors,
        };
    }
}
