using System.ComponentModel.DataAnnotations;

namespace Hippo.Application.Revisions.Queries;

public class RevisionDetailsVm
{
    [Required]
    public Guid Id { get; set; }

    [Required]
    public string? RevisionNumber { get; set; }

    [Required]
    public string? Description { get; set; }

    [Required]
    public string? Type { get; set; }

    [Required]
    public string? Base { get; set; }

    public List<RevisionComponentDto> Components { get; set; } = new List<RevisionComponentDto>();
}
