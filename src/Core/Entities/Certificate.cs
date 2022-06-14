namespace Hippo.Core.Entities;

public class Certificate : AuditableEntity
{
    public string? Name { get; set; }

    public string? PublicKey { get; set; }

    public string? PrivateKey { get; set; }

    public IList<Channel> Channels { get; private set; } = new List<Channel>();
}
