namespace Hippo.Core.Entities;

public class Revision : AuditableEntity, IHasDomainEvent
{
    public Guid Id { get; set; }

    // This is the revision number that gets composed with the Application.StorageId
    // to get the bindle ID.  E.g. this might be "1.4.0" or "1.1.5-prerelease2".
    public string? RevisionNumber { get; set; }

    public Guid AppId { get; set; }

    public App App { get; set; } = null!;

    public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
}
