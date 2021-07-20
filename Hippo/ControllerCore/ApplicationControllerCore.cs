using Hippo.Models;
using Hippo.Repositories;
using Hippo.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Hippo.ControllerCore
{
    public abstract class ApplicationControllerCore : HippoController
    {
        private protected readonly IUnitOfWork _unitOfWork;
        private protected readonly UserManager<Account> _userManager;
        private protected readonly ITaskQueue<ChannelReference> _channelsToReschedule;
        private protected readonly EventOrigin _eventSource;

        protected ApplicationControllerCore(IUnitOfWork unitOfWork, UserManager<Account> userManager, ITaskQueue<ChannelReference> channelsToReschedule, ILogger logger, EventOrigin eventSource)
            : base(logger)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _channelsToReschedule = channelsToReschedule;
            _eventSource = eventSource;
        }
    }
}
