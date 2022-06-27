using System.ComponentModel.DataAnnotations;

namespace Hippo.Application.Common.Interfaces.StorageService;

public class RevisionComponent
{
    /// The module source.
    [Required]
    public string Source { get; set; } = string.Empty;

    /// ID of the component. Used at runtime to select between
    /// multiple components of the same application.
    [Required]
    public string Id { get; set; } = string.Empty;

    /// Description of the component.
    public string? Description { get; set; }

    /// The parcel group to be mapped inside the Wasm module at runtime.
    public string? Files { get; set; }

    /// Optional list of HTTP hosts the component is allowed to connect.
    public List<string>? AllowedHTTPHosts { get; set; }

    /// Environment variables to be mapped inside the Wasm module at runtime.
    public Dictionary<string, string>? Environment { get; set; }

    /// Trigger configuration.
    public RevisionComponentTrigger? Trigger { get; set; }

    /// Component-specific configuration values.
    public Dictionary<string, string>? Config { get; set; }
}
