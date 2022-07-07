using System.ComponentModel.DataAnnotations;
using Hippo.Application.Common.Mappings;
using Hippo.Core.Entities;

namespace Hippo.Application.EnvironmentVariables.Queries;

public class EnvironmentVariableItem : IMapFrom<EnvironmentVariable>
{
    [Required]
    public Guid ChannelId { get; set; }

    [Required]
    public string Key { get; set; } = "";

    [Required]
    public string Value { get; set; } = "";
}
