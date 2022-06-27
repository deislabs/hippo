namespace Hippo.Web.Extensions;

public class HealthInfo
{
	public string ServiceName { get; set; } = string.Empty;
	public string Description { get; set; } = string.Empty;
	public string Status { get; set; } = string.Empty;
	public List<HealthInfo> Subservices { get; set; } = new List<HealthInfo>();
}
