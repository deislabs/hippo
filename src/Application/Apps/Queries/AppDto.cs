using System.ComponentModel.DataAnnotations;
using Hippo.Application.Channels.Queries;
using Hippo.Application.Common.Mappings;
using Hippo.Application.Revisions.Queries;
using Hippo.Core.Entities;

namespace Hippo.Application.Apps.Queries;

public class AppDto : IMapFrom<App>
{
    public AppDto()
    {
        Channels = new List<ChannelDto>();
        Revisions = new List<RevisionDto>();
    }

    [Required]
    public Guid Id { get; set; }

    [Required]
    public string Name { get; set; } = "";

    [Required]
    public string StorageId { get; set; } = "";

    [Required]
    public IList<ChannelDto> Channels { get; set; }

    [Required]
    public IList<RevisionDto> Revisions { get; set; }
}
