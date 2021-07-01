
using System.Collections.Generic;
using System.Threading;
using Microsoft.Extensions.Primitives;
using Yarp.ReverseProxy.Configuration;

namespace Hippo.Config
{
    public class ChannelConfig : IProxyConfig
    {
        private readonly IReadOnlyList<RouteConfig> _routes;
        private readonly IReadOnlyList<ClusterConfig> _clusters;
        private readonly CancellationTokenSource _cancellationTokenSource = new();
        public IReadOnlyList<RouteConfig> Routes => _routes;
        public IReadOnlyList<ClusterConfig> Clusters => _clusters;
        public IChangeToken ChangeToken { get; }

        public ChannelConfig(IReadOnlyList<RouteConfig> routes = null, IReadOnlyList<ClusterConfig> clusters = null)
        {
            _routes = routes ?? new List<RouteConfig>();
            _clusters = clusters ?? new List<ClusterConfig>();
            ChangeToken = new CancellationChangeToken(_cancellationTokenSource.Token);
        }

        internal void SignalChange()
        {
            _cancellationTokenSource.Cancel();
        }

    }
}
