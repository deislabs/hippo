using System;
using System.Threading.Tasks;

namespace Hippo.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IAccountRepository Accounts { get; }
        IApplicationRepository Applications { get; }
        IChannelRepository Channels { get; }
        IEventLogRepository EventLog { get; }
        IRevisionRepository Revisions { get; }

        Task SaveChanges();
    }
}
