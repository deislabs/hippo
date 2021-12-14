namespace Hippo.Core.Events;

public class AppCreatedEvent : DomainEvent
{
    public AppCreatedEvent(App app)
    {
        App = app;
    }

    public App App { get; }
}
