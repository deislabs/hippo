using Hippo.Application.Jobs;

namespace Hippo.Application.Common.Interfaces;

public interface IBindleService
{
    public Task<List<string?>> QueryAvailableStorages(string query, ulong? offset, int? limit);
}
