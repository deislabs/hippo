using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Hippo.Application.Common.Interfaces.StorageService;

public class RevisionSpinToml
{
    public RevisionTrigger? Trigger { get; set; }

    public Dictionary<string, Dictionary<string, string>> Config { get; set; } = new();

    [Required]
    [DataMember(Name = "component")]
    public List<RevisionComponent> Components { get; set; } = new();
}
