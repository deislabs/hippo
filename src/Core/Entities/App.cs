namespace Hippo.Core.Entities;

public class App : AuditableEntity, IHasDomainEvent
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    // This is the ID in Bindle or whatever storage backend is used.  It gets composed
    // with a revision ID to get a Bindle ID.
    //
    // For example, the Weather application might have the StorageId contoso/weather.
    // Revision 1.4.0 of the Weather application would then have the bindle id
    // contoso/weather/1.4.0
    public string? StorageId { get; set; }

    public IList<Channel> Channels { get; private set; } = new List<Channel>();

    public IList<Revision> Revisions { get; private set; } = new List<Revision>();

    public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
}
