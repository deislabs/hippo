using System.ComponentModel.DataAnnotations;

namespace Hippo.Application.Revisions.Queries;

public class RevisionDetailsDto
{
    [Required]
    public string Name { get; set; } = "";

    [Required]
    public string Version { get; set; } = "";

    public string? Description { get; set; } = null;

    [Required]
    public IEnumerable<string> Authors { get; set; } = new List<string>();
}
