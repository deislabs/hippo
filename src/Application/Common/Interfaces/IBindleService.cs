using Hippo.Application.Revisions.Queries;

namespace Hippo.Application.Common.Interfaces;

public interface IBindleService
{
    public Task<RevisionDetails> GetRevisionDetails(string revisionId);

    public Task<IEnumerable<string>> GetBindleRevisionNumbers(string bindleId);

    public Task<IEnumerable<string>> QueryAvailableStorages(string query, ulong? offset, int? limit);
}
