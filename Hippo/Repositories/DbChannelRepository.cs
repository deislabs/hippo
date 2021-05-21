using System.Linq;
using System.Threading.Tasks;
using Hippo.Models;
using Microsoft.EntityFrameworkCore;

namespace Hippo.Repositories
{
    public class DbChannelRepository : IChannelRepository
    {
        private readonly DataContext _context;

        public DbChannelRepository(DataContext context)
        {
            _context = context;
        }

        public Channel GetChannelByName(Application owner, string name) =>
            _context.Channels
                    .Where(c => c.Application == owner && c.Name == name)
                    .Include(c => c.Application)
                    .Include(c => c.Configuration)
                        .ThenInclude(c => c.EnvironmentVariables)
                    .Include(c => c.Domain)
                    .Include(c => c.Release)
                    .SingleOrDefault();
    }
}