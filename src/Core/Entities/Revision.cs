namespace Hippo.Core.Entities;

public class Revision : AuditableEntity
{
    // This is the revision number that gets composed with the Application.StorageId
    // to get the bindle ID.  E.g. this might be "1.4.0" or "1.1.5-prerelease2".
    public string? RevisionNumber { get; set; }

    public string? Description { get; set; }

    public Guid AppId { get; set; }

    public App App { get; set; } = null!;

    public IList<RevisionComponent> Components { get; private set; } = new List<RevisionComponent>();
}
