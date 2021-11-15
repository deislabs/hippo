using System.Collections.Generic;
using System.Linq;

namespace Hippo.Models;

public enum HealthLevel
{
    Healthy,
    Unhealthy,
}

public class HealthStatus
{
    public HealthLevel Health { get; }
    public IReadOnlyCollection<string> Messages { get; }

    public static readonly HealthStatus Healthy = new HealthStatus(HealthLevel.Healthy);

    public static HealthStatus Unhealthy(string message) =>
        new HealthStatus(HealthLevel.Unhealthy, new[] { message });

    public HealthStatus(HealthLevel health)
        : this(health, Enumerable.Empty<string>()) { }

    public HealthStatus(HealthLevel health, IEnumerable<string> messages)
    {
        Health = health;
        Messages = new List<string>(messages).AsReadOnly();
    }
}
