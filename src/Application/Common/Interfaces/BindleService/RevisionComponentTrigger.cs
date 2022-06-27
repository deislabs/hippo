using System.ComponentModel.DataAnnotations;

namespace Hippo.Application.Common.Interfaces.StorageService;

public class RevisionComponentTrigger
{
    public string? Route { get; set; }

    public string? Channel { get; set; }
}
