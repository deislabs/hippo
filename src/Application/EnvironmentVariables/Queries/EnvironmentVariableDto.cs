using Hippo.Application.Common.Mappings;
using Hippo.Core.Entities;

namespace Hippo.Application.EnvironmentVariables.Queries;

public class EnvironmentVariableDto : IMapFrom<EnvironmentVariable>
{
    public Guid Id { get; set; }

    public Guid ChannelId { get; set; }

    public string? Key { get; set; }

    public string? Value { get; set; }
}
