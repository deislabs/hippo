using System.ComponentModel.DataAnnotations;

namespace Hippo.Application.Revisions.Queries;

public class RevisionTrigger
{
    [Required]
    public string Type { get; set; } = string.Empty;

    [Required]
    public string Base { get; set; } = string.Empty;
}
