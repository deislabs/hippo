using Hippo.Application.Common.Mappings;
using Hippo.Core.Entities;

namespace Hippo.Application.Revisions.Queries;

public class RevisionRecord : IMapFrom<Revision>
{
    public string? RevisionNumber { get; set; }
}
