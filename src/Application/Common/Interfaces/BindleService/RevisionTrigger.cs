using System.ComponentModel.DataAnnotations;

namespace Hippo.Application.Common.Interfaces.BindleService;

public class RevisionTrigger
{
    [Required]
    public string Type { get; set; } = string.Empty;

    [Required]
    public string Base { get; set; } = string.Empty;
}
