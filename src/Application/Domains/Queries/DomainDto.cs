using Hippo.Application.Common.Mappings;
using Hippo.Core.Entities;

namespace Hippo.Application.Domains.Queries;

public class DomainDto : IMapFrom<Domain>
{
    public Guid Id { get; set; }

    public Guid ChannelId { get; set; }

    public string? Name { get; set; }
}
