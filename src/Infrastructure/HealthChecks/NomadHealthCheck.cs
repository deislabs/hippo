using Fermyon.Nomad.Api;
using Fermyon.Nomad.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Hippo.Infrastructure.HealthChecks;

public class NomadHealthCheck : IHealthCheck
{
    private StatusApi _client;

    private readonly IConfiguration _configuration;

    public NomadHealthCheck(IConfiguration configuration)
    {
        _configuration = configuration;
        var nomadUrl = configuration.GetValue("Nomad:Url", "http://localhost:4646/v1");
        var nomadSecret = configuration.GetValue("Nomad:Secret", "");

        Configuration config = new Configuration();
        config.BasePath = nomadUrl;
        config.ApiKey.Add("X-Nomad-Token", nomadSecret);

        _client = new StatusApi(config);
    }

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            // TODO: implement a smarter health check (check if node members are alive and healthy)
            //
            // For now just check that we can make requests against the API
            _client.GetStatusLeader();
            return Task.FromResult(HealthCheckResult.Healthy("A healthy result."));
        }
        catch (Exception)
        {
            return Task.FromResult(new HealthCheckResult(context.Registration.FailureStatus, "An unhealthy result."));
        }
    }
}
