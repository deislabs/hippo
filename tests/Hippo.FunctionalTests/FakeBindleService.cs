using Hippo.Application.Common.Interfaces;
using Hippo.Application.Revisions.Queries;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hippo.FunctionalTests;

internal class FakeBindleService : IBindleService
{
    public async Task<RevisionDetails> GetRevisionDetails(string revisionId)
    {
        return await Task.FromResult(new RevisionDetails());
    }

    public async Task<IEnumerable<string>> GetBindleRevisionNumbers(string bindleId)
    {
        return await Task.FromResult(new List<string> { "1.0.0" });
    }
    
    public async Task<IEnumerable<string>> QueryAvailableStorages(string query, ulong? offset, int? limit)
    {
        return await Task.FromResult(new List<string> { query });
    }
}
