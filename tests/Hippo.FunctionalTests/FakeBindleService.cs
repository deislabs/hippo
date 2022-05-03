using Hippo.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hippo.FunctionalTests
{
    internal class FakeBindleService : IBindleService
    {
        public async Task<IEnumerable<string>> QueryAvailableStorages(string query, ulong? offset, int? limit)
        {
            return await Task.FromResult(new List<string> { query });
        }
    }
}
