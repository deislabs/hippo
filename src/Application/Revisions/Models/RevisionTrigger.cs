using System.ComponentModel.DataAnnotations;

namespace Hippo.Application.Revisions.Models;

public class RevisionTrigger
{
    [Required]
    public string Type { get; set; } = string.Empty;

    [Required]
    public string Base { get; set; } = string.Empty;
}
