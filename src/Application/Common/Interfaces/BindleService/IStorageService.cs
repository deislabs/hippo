using Hippo.Application.Common.Interfaces.StorageService;

namespace Hippo.Application.Common.Interfaces;

public interface IStorageService
{
    public Task<RevisionDetails> GetRevisionDetails(string revisionId);

    public Task<IEnumerable<string>> GetBindleRevisionNumbers(string bindleId);

    /// <summary>
    /// Returns a list of unique storage IDs available on the storage server that match the given "query" string.
    /// </summary>
    /// <param name="limit">The maximum number of unique storage IDs to be returned</param>
    /// <param name="offset">Offset used for pagination</param>
    public Task<IEnumerable<string>> QueryAvailableStorages(string query, ulong? offset, int? limit);
}
