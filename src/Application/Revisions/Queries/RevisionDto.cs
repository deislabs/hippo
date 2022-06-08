using System.ComponentModel.DataAnnotations;
using Hippo.Application.Common.Mappings;
using Hippo.Core.Entities;

namespace Hippo.Application.Revisions.Queries;

public class RevisionDto : IMapFrom<Revision>
{
    [Required]
    public Guid Id { get; set; }

    [Required]
    public Guid AppId { get; set; }

    [Required]
    public string RevisionNumber { get; set; } = "";

    [Required]
    public IList<RevisionComponentDto> Components { get; private set; } = new List<RevisionComponentDto>();

    public string OrderKey()
    {
        if (SemVer.Version.TryParse(RevisionNumber, out var version))
        {
            return $"{version.Major:D9}{version.Minor:D9}{version.Patch:D9}{RevisionNumber}";
        }
        return RevisionNumber!;
    }
}
