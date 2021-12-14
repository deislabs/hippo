namespace Hippo.Core.Events;

public class AppDeletedEvent : DomainEvent
{
    public AppDeletedEvent(App app)
    {
        App = app;
    }

    public App App { get; }
}
