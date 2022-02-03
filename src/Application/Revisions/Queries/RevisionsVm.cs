using System.ComponentModel.DataAnnotations;

namespace Hippo.Application.Revisions.Queries;

public class RevisionsVm
{
    [Required]
    public IList<RevisionDto> Revisions { get; set; } = new List<RevisionDto>();
}
