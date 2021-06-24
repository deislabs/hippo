using System;
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
                    .Include(c => c.ActiveRevision)
                    .Include(c => c.SpecifiedRevision)
                    .SingleOrDefault();

        public Channel GetChannelById(Guid id) =>
            _context.Channels
                    .Where(c => c.Id == id)
                    .Include(c => c.Application)
                    .Include(c => c.Configuration)
                        .ThenInclude(c => c.EnvironmentVariables)
                    .Include(c => c.Domain)
                    .Include(c => c.ActiveRevision)
                    .Include(c => c.SpecifiedRevision)
                    .SingleOrDefault();

        public async Task AddNew(Channel channel)
        {
            // TODO: remove once we sort out the ports stuff
            try
            {
                var lastPort = _context.Channels.Max(c => c.PortID);
                channel.PortID = lastPort + 1;
            }
            catch (InvalidOperationException)
            {
                channel.PortID = 0;
            }
            await _context.Channels.AddAsync(channel);
        }
    }
}
