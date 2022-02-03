using Hippo.Application.Common.Mappings;
using Hippo.Core.Entities;

namespace Hippo.Application.EnvironmentVariables.Queries;

public class EnvironmentVariableRecord : IMapFrom<EnvironmentVariable>
{
    public string Key { get; set; } = "";

    public string Value { get; set; } = "";
}
