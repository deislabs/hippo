using Hippo.Application.Common.Mappings;
using Hippo.Core.Entities;
using System.ComponentModel.DataAnnotations;

namespace Hippo.Application.EnvironmentVariables.Commands;
public class UpdateEnvironmentVariableDto
{
    [Required]
    public Guid? Id { get; set; }

    [Required]
    public string Key { get; set; } = "";

    [Required]
    public string Value { get; set; } = "";

    [Required]
    public Guid ChannelId { get; set; }
}