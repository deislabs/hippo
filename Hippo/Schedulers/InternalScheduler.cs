using System.Collections.Generic;
using Hippo.Messages;
using Hippo.Models;
using Hippo.Proxies;
using Microsoft.Extensions.Logging;

namespace Hippo.Schedulers
{
    public abstract class InternalScheduler : IJobScheduler
    {
        private protected readonly ILogger _logger;
        private protected readonly IReverseProxy _reverseProxy;

        private protected InternalScheduler(ILogger logger, IReverseProxy reverseProxy)
        {
            _logger = logger;
            _reverseProxy = reverseProxy;
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

        private protected virtual void StartProxy(Channel channel, string address)
        {
            _reverseProxy.StartProxy(channel, address);
        }

        private protected virtual void StopProxy(Channel channel)
        {
            _reverseProxy.StopProxy(channel);
        }

    }
}
