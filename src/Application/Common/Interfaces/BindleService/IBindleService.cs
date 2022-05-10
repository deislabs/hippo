using Hippo.Application.Common.Interfaces.BindleService;

namespace Hippo.Application.Common.Interfaces;

public interface IBindleService
{
    public Task<RevisionDetails> GetRevisionDetails(string revisionId);

    public Task<IEnumerable<string>> GetBindleRevisionNumbers(string bindleId);

    public Task<IEnumerable<string>> QueryAvailableStorages(string query, ulong? offset, int? limit);
}
