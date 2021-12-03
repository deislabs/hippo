namespace Hippo.Core.Common;

public abstract class AuditableEntity
{

    protected AuditableEntity()
    {
        Created = DateTime.UtcNow;
        LastModified = DateTime.UtcNow;
    }

    public DateTime Created { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime LastModified { get; set; }

    public string? LastModifiedBy { get; set; }

}
