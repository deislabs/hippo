using System.ComponentModel.DataAnnotations;

namespace Hippo.Application.Common.Interfaces.StorageService;

public class RevisionDetails
{
    [Required]
    public string? Name { get; set; }

    [Required]
    public string? Version { get; set; }

    public string? Description { get; set; }

    [Required]
    public IEnumerable<string> Authors { get; set; } = new List<string>();

    [Required]
    public RevisionSpinToml? SpinToml { get; set; }
}
