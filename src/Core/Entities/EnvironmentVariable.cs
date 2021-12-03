namespace Hippo.Core.Entities;

public class EnvironmentVariable : AuditableEntity, IHasDomainEvent
{
    public Guid Id { get; set; }

    public string? Key { get; set; }

    public string? Value { get; set; }

    public Guid ChannelId { get; set; }

    public Channel Channel { get; set; } = null!;

    public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
}
