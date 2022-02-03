using System.ComponentModel.DataAnnotations;

namespace Hippo.Application.EnvironmentVariables.Queries;

public class EnvironmentVariablesVm
{
    [Required]
    public IList<EnvironmentVariableDto> EnvironmentVariables { get; set; } = new List<EnvironmentVariableDto>();
}
