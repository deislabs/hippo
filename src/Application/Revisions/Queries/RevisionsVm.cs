namespace Hippo.Application.Revisions.Queries;

public class RevisionsVm
{
    public IList<RevisionDto> Revisions { get; set; } = new List<RevisionDto>();
}
