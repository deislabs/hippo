using System.ComponentModel.DataAnnotations;

namespace Hippo.Application.Revisions.Queries;

public class RevisionComponentTrigger
{
    [Required]
    public string Route { get; set; } = string.Empty;
}
