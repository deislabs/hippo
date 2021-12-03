using Hippo.Application.Common.Mappings;
using Hippo.Core.Entities;

namespace Hippo.Application.Domains.Queries;

public class DomainRecord : IMapFrom<Domain>
{
    public string? Name { get; set; }
}
