using System;
using System.Threading.Tasks;
using Hippo.Models;

namespace Hippo.Repositories
{
    public class DbUnitOfWork : IUnitOfWork
    {
        private readonly DataContext _dataContext;

        public DbUnitOfWork(DataContext dataContext, ICurrentUser currentUser)
        {
            _dataContext = dataContext;

            Accounts = new DbAccountRepository(_dataContext);
            Applications = new DbApplicationRepository(_dataContext, currentUser);
            Channels = new DbChannelRepository(_dataContext);
            Revisions = new DbRevisionRepository(_dataContext);
        }

        // We could make these lazy, but they are as cheap to construct as a Lazy would
        // be, so why bother
        public IAccountRepository Accounts { get; }
        public IApplicationRepository Applications { get; }
        public IChannelRepository Channels { get; }
        public IRevisionRepository Revisions { get; }

        public async Task SaveChanges() => await _dataContext.SaveChangesAsync();

        public void Dispose()
        {
            _dataContext.Dispose();
        }
    }
}