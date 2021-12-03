namespace Hippo.Application.EnvironmentVariables.Queries;

public class EnvironmentVariablesVm
{
    public IList<EnvironmentVariableDto> EnvironmentVariables { get; set; } = new List<EnvironmentVariableDto>();
}
