using System.ComponentModel.DataAnnotations;

namespace Hippo.Application.Revisions.Models;

public class RevisionComponentTrigger
{
    [Required]
    public string Route { get; set; } = string.Empty;
}
