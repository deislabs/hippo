using System.ComponentModel.DataAnnotations;
using Hippo.Application.Common.Mappings;
using Hippo.Core.Entities;

namespace Hippo.Application.EnvironmentVariables.Queries;

public class EnvironmentVariableDto : IMapFrom<EnvironmentVariable>
{
    [Required]
    public Guid Id { get; set; }

    [Required]
    public Guid ChannelId { get; set; }

    [Required]
    public string Key { get; set; } = "";

    [Required]
    public string Value { get; set; } = "";
}
