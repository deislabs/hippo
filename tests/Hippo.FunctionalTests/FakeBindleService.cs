using Hippo.Application.Common.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hippo.FunctionalTests;

internal class FakeBindleService : IBindleService
{
    public async Task<IEnumerable<string>> GetBindleRevisionNumbers(string bindleId)
    {
        return await Task.FromResult(new List<string> { "1.0.0" });
    }
}
