using System.Linq;
using System.Threading.Tasks;
using Hippo.Models;

namespace Hippo.Repositories
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
