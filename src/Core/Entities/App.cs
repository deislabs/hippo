namespace Hippo.Core.Entities;

public class App : AuditableEntity, IHasDomainEvent
{
    public Guid Id { get; set; }

    // The name of the application.
    //
    // This doubles as the Bindle storage identifier. It is composed
    // with a revision number to resolve the full Bindle id.
    //
    // For example, an application might have the Name contoso/weather.
    // Revision 1.4.0 of the contoso/weather application would then have the Bindle id
    // contoso/weather/1.4.0
    public string? Name { get; set; }

    public IList<Channel> Channels { get; private set; } = new List<Channel>();

    public IList<Revision> Revisions { get; private set; } = new List<Revision>();

    public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
}
