namespace Hippo.Core.Events;

public class EnvironmentVariableCreatedEvent : DomainEvent
{
    public EnvironmentVariableCreatedEvent(EnvironmentVariable environmentVariable)
    {
        EnvironmentVariable = environmentVariable;
    }

    public EnvironmentVariable EnvironmentVariable { get; }
}
