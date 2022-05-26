using System.ComponentModel.DataAnnotations;

namespace Hippo.Application.Common.Interfaces.BindleService;

public class RevisionComponentTrigger
{
    [Required]
    public string? Route { get; set; }

    [Required]
    public string? Channel { get; set; }
}
