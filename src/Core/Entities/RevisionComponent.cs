namespace Hippo.Core.Entities;

public class RevisionComponent : AuditableEntity
{
    public string Source { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string? Route { get; set; }

    public string? Channel { get; set; }

    public string? Type { get; set; }

    public Guid RevisionId { get; set; }

    public Revision Revision { get; set; } = null!;
}
