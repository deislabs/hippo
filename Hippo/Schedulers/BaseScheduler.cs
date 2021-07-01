using System.Collections.Generic;
using Hippo.Messages;
using Hippo.Models;
using Hippo.Providers;
using Microsoft.Extensions.Logging;

namespace Hippo.Schedulers
{
    public abstract class BaseScheduler : IJobScheduler
    {
        private protected readonly ILogger _logger;
        private protected readonly IProxyConfigUpdater _proxyConfigUpdater;

        private protected BaseScheduler(ILogger<WagiLocalJobScheduler> logger, IProxyConfigUpdater proxyConfigUpdater)

        {
            _logger = logger;
            _proxyConfigUpdater = proxyConfigUpdater;
        }

        public virtual void OnSchedulerStart(IEnumerable<Application> applications)
        {
            foreach (var application in applications)
            {
                foreach (var channel in application.Channels)
                {
                    _logger.LogTrace($"Starting Channel:{channel.Id} Application: {application.Id}");
                    Start(channel);
                }
            }
        }

        public abstract void Start(Channel c);

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1716:Identifiers should not match keywords", Justification = "Don't care about VB")]
        public abstract void Stop(Channel c);

        public virtual void UpdateYarp(YarpConfigurationRequest request)
        {
            _proxyConfigUpdater.UpdateConfig(request);
        }

    }
}
