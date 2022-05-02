using Hippo.Application.Jobs;

namespace Hippo.Application.Common.Interfaces;

public interface IBindleService
{
    public Task<IEnumerable<string>> QueryAvailableStorages(string query, ulong? offset, int? limit);
}
