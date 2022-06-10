using Hippo.Application.Common.Mappings;
using System.ComponentModel.DataAnnotations;

namespace Hippo.Application.Revisions.Queries;

public class RevisionComponentDto : IMapFrom<Core.Entities.RevisionComponent>
{
    [Required]
    public Guid Id { get; set; }

    [Required]
    public string Source { get; set; } = string.Empty;

    [Required]
    public string Name { get; set; } = string.Empty;

    public string? Route { get; set; }

    public string? Channel { get; set; }

    public string? Type { get; set; }
}
