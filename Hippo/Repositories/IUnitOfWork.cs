using System.Threading.Tasks;

namespace Hippo.Repositories
{
    public interface IUnitOfWork
    {
        IAccountRepository Accounts { get; }
        IApplicationRepository Applications { get; }
        IChannelRepository Channels { get; }
        IReleaseRepository Releases { get; }

        Task SaveChanges();
    }
}
