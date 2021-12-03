namespace Hippo.Core.Events;

public class EnvironmentVariableDeletedEvent : DomainEvent
{
    public EnvironmentVariableDeletedEvent(EnvironmentVariable environmentVariable)
    {
        EnvironmentVariable = environmentVariable;
    }

    public EnvironmentVariable EnvironmentVariable { get; }
}
