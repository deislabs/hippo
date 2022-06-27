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

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            await _client.QueryInvoices();
            return HealthCheckResult.Healthy("A healthy result.");
        }
        catch (Exception)
        {
            return new HealthCheckResult(context.Registration.FailureStatus, "An unhealthy result.");
        }
    }
}
