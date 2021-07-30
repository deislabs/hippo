using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Hippo.Config;
using Hippo.Models;
using Hippo.Proxies;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Hippo.Schedulers
{
    public class WagiDotnetJobScheduler : InternalScheduler
    {
        private readonly IChannelConfigurationProvider _channelConfigurationProvider;
        public WagiDotnetJobScheduler(ILogger<WagiDotnetJobScheduler> logger, IReverseProxy reverseProxy, IChannelConfigurationProvider channelConfigurationProvider)
            : base(logger, reverseProxy)
        {
            _channelConfigurationProvider = channelConfigurationProvider;
            _channelConfigurationProvider.SetBindleServer(_bindleUrl);
        }

        public override void Start(Channel c)
        {
            var port = c.PortID + Channel.EphemeralPortRange;
            var listenAddress = $"http://127.0.0.1:{port}";
            _channelConfigurationProvider.AddChannel(c, listenAddress);
            StartProxy(c, listenAddress);
        }

        public override void Stop(Channel c)
        {
            StopProxy(c);
            _channelConfigurationProvider.RemoveChannel(c);

        }
    }
}
