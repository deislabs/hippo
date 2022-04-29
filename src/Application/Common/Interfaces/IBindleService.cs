using Hippo.Application.Jobs;
using Hippo.Application.Revisions.Queries;

namespace Hippo.Application.Common.Interfaces;

public interface IBindleService
{
    public Task<RevisionDetailsDto> GetRevisionDetails(string revisionId);
}
