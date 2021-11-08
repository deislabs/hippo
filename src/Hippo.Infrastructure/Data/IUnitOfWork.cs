using System;
using System.Threading.Tasks;

namespace Hippo.Infrastructure.Data
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
