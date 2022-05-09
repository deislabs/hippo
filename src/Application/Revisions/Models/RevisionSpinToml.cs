using System.ComponentModel.DataAnnotations;

namespace Hippo.Application.Revisions.Models;

public class RevisionSpinToml
{
    [Required]
    public RevisionTrigger? Trigger { get; set; }

    [Required]
    public List<RevisionComponent> Component { get; set; } = new();
}
