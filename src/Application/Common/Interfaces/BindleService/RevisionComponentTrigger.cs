using System.ComponentModel.DataAnnotations;

namespace Hippo.Application.Common.Interfaces.BindleService;

public class RevisionComponentTrigger
{
    public string? Route { get; set; }

    public string? Channel { get; set; }
}
