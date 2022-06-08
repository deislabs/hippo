using Deislabs.Bindle;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Hippo.Infrastructure.HealthChecks;

public class BindleHealthCheck : IHealthCheck
{
    private BindleClient _client { get; set; }

    public BindleHealthCheck(IConfiguration configuration)
    {
        _client = new BindleClient(configuration.GetConnectionString("Bindle"));
    }

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            _client.QueryInvoices();
            return Task.FromResult(HealthCheckResult.Healthy("A healthy result."));
        }
        catch (Exception)
        {
            return Task.FromResult(new HealthCheckResult(context.Registration.FailureStatus, "An unhealthy result."));
        }
    }
}
