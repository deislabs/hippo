using System.ComponentModel.DataAnnotations;

namespace Hippo.Application.Common.Interfaces.StorageService;

public class RevisionTrigger
{
    [Required]
    public string Type { get; set; } = string.Empty;

    public string? Base { get; set; }

    public string? Address { get; set; }
}
