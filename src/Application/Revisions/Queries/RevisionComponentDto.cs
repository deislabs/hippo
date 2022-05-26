using Hippo.Application.Common.Mappings;
using Hippo.Core.Enums;
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

    [Required]
    public string? Route { get; set; }

    [Required]
    public string? Channel { get; set; }

    [Required]
    public RevisionComponentType? Type { get; set; }
}
