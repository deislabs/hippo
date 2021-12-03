namespace Hippo.Core.Entities;

public class Domain : AuditableEntity, IHasDomainEvent
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public Guid ChannelId { get; set; }

    public Channel Channel { get; set; } = null!;

    public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
}
