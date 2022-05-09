using System.ComponentModel.DataAnnotations;

namespace Hippo.Application.Revisions.Models;

public class RevisionComponent
{
    [Required]
    public string Source { get; set; } = string.Empty;

    [Required]
    public string Id { get; set; } = string.Empty;

    [Required]
    public RevisionComponentTrigger? Trigger { get; set; }
}
