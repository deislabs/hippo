namespace Hippo.Core.Entities;

public class EnvironmentVariable : AuditableEntity
{
    public string? Key { get; set; }

    public string? Value { get; set; }

    public Guid ChannelId { get; set; }

    public Channel Channel { get; set; } = null!;
}
