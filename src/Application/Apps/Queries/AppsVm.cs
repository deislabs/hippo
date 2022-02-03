using System.ComponentModel.DataAnnotations;

namespace Hippo.Application.Apps.Queries;

public class AppsVm
{
    [Required]
    public IList<AppDto> Apps { get; set; } = new List<AppDto>();
}
