using System.ComponentModel.DataAnnotations;
using Hippo.Application.Common.Attributes;
using Hippo.Application.Common.Mappings;
using Hippo.Core.Entities;

namespace Hippo.Application.Revisions.Queries;

public class RevisionItem : IMapFrom<Revision>, IComparable<RevisionItem>
{
    [Required]
    public Guid Id { get; set; }

    [Required]
    public Guid AppId { get; set; }

    [Required]
    public string RevisionNumber { get; set; } = "";

    [Required]
    public IList<RevisionComponentDto> Components { get; private set; } = new List<RevisionComponentDto>();

    [NoMap]
    public string? Type { get; set; }

    public int CompareTo(RevisionItem? other)
    {
        if (other is null)
            throw new ArgumentNullException();

        Version a = new Version(RevisionNumber);
        Version b = new Version(other.RevisionNumber);
        return a.CompareTo(b);
    }
}
