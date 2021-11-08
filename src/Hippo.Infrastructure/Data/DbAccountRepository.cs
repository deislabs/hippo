using System.Linq;
using Hippo.Infrastructure.Data;

namespace Hippo.Infrastructure.Data
{
    public class DbAccountRepository : IAccountRepository
    {
        private readonly DataContext _context;

        public DbAccountRepository(DataContext context)
        {
            _context = context;
        }

        public bool IsEmpty() =>
            !_context.Accounts.Any();
    }
}
