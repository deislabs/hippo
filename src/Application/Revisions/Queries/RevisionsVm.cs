using System.ComponentModel.DataAnnotations;

namespace Hippo.Application.Revisions.Queries;

public class RevisionsVm
{
    [Required]
    public IList<RevisionItem> Revisions { get; set; } = new List<RevisionItem>();
}
