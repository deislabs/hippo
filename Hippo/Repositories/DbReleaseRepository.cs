using System.Linq;
using Hippo.Models;

namespace Hippo.Repositories
{
    public class DbReleaseRepository: IReleaseRepository
    {
        private readonly DataContext _context;

        public DbReleaseRepository(DataContext context)
        {
            _context = context;
        }

        public Release GetReleaseByRevision(Application owner, string revision) =>
            _context.Releases
                    .Where(r => r.Application == owner && r.Revision == revision)
                    .SingleOrDefault();
    }
}