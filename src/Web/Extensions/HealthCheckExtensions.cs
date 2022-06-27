using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Newtonsoft.Json;
using System.Net.Mime;

namespace Hippo.Web.Extensions;

public static class HealthCheckExtensions
{
	public static IEndpointConventionBuilder MapCustomHealthChecks(
		this IEndpointRouteBuilder endpoints)
	{
		return endpoints.MapHealthChecks("/healthz", new HealthCheckOptions
		{
			ResultStatusCodes =
			{
				[HealthStatus.Healthy] = StatusCodes.Status200OK,
				[HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
			},
			ResponseWriter = async (context, report) =>
			{
				var result = JsonConvert.SerializeObject(
					new HealthInfo
                    {
						ServiceName = "Hippo",
						Status = report.Status.ToString(),
						Subservices = new List<HealthInfo>(
							report.Entries.Select(e => new HealthInfo
							{
								ServiceName = e.Key ?? string.Empty,
								Description = e.Value.Description ?? string.Empty,
								Status = Enum.GetName(typeof(HealthStatus), e.Value.Status)
												?? string.Empty,
							})
						)
					}
				);
				context.Response.ContentType = MediaTypeNames.Application.Json;
				await context.Response.WriteAsync(result);
			}
		});
	}
}
