using System;
using System.Collections.Generic;
using Hippo.Models;
using Hippo.Proxies;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Hippo.Schedulers
{
    public abstract class InternalScheduler : IJobScheduler
    {
        private protected readonly ILogger _logger;
        private protected readonly IReverseProxy _reverseProxy;
        private protected readonly string _bindleUrl;
        private const string ENV_BINDLE = "BINDLE_URL";

        private protected InternalScheduler(ILogger logger, IReverseProxy reverseProxy, IHostEnvironment env)
        {
            _logger = logger;
            _reverseProxy = reverseProxy;

            _bindleUrl = Environment.GetEnvironmentVariable(ENV_BINDLE);
            if (string.IsNullOrWhiteSpace(_bindleUrl))
            {
                _logger.LogError($"Bindle server URL not specified: set {ENV_BINDLE} environment variable");
                if (!env.IsDevelopment())
                {
                    throw new ArgumentException($"No Channels will be able to run - this scheduler requires {ENV_BINDLE} environment variable to run");
                }
            }
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
