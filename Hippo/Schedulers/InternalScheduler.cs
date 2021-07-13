using System;
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
                    if (channel.ActiveRevision == null)
                    {
                        _logger.LogWarning($"Scheduler start: Skipping channel {channel.Name} in application {application.Name}: no active revision");
                    }
                    else
                    {
                        _logger.LogTrace($"Scheduler start: Starting channel {channel.Name} in application {application.Name}");
                        try
                        {
                            Start(channel);
                        }
                        catch (Exception e)
                        {
                            _logger.LogWarning($"Scheduler start: Error starting channel {channel.Name} in application {application.Name}: {e}");
                        }
                    }
                }
            }
        }

        public abstract void Start(Channel c);

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1716:Identifiers should not match keywords", Justification = "Don't care about VB")]
        public abstract void Stop(Channel c);

        public virtual void StartProxy(Channel channel, string address)
        {
            _reverseProxy.StartProxy(channel, address);
        }

        public virtual void StopProxy(Channel channel)
        {
            _reverseProxy.StopProxy(channel);
        }

    }
}
