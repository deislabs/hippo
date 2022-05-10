using Hippo.Application.Common.Mappings;
using Hippo.Core.Entities;
using System.ComponentModel.DataAnnotations;

namespace Hippo.Application.Revisions.Queries;

public class RevisionDetailsDto : IMapFrom<Revision>
{
    [Required]
    public Guid Id { get; set; }

    [Required]
    public string? RevisionNumber { get; set; }

    [Required]
    public string? Description { get; set; }

    public IList<RevisionComponentDto> Components { get; private set; } = new List<RevisionComponentDto>();
}
