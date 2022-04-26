using Hippo.Core.Entities;

namespace Hippo.Application.Common.Interfaces;

public interface IBindleService
{
    public Task<List<string>> GetBindleRevisionNumbers(string bindleId);
}
