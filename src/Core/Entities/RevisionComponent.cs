namespace Hippo.Core.Entities;

public class RevisionComponent : AuditableEntity, IHasDomainEvent
{
    public Guid Id { get; set; }

    public string Source { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Route { get; set; } = string.Empty;

    public Guid RevisionId { get; set; }

    public Revision Revision { get; set; } = null!;

    public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
}
