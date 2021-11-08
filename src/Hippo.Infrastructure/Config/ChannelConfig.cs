
using System.Collections.Generic;
using System.Threading;
using Microsoft.Extensions.Primitives;
using Yarp.ReverseProxy.Configuration;

namespace Hippo.Infrastructure.Config
{
    public class ChannelConfig : IProxyConfig
    {
        private readonly CancellationTokenSource _cancellationTokenSource = new();
        public IReadOnlyList<RouteConfig> Routes { get; }
        public IReadOnlyList<ClusterConfig> Clusters { get; }
        public IChangeToken ChangeToken { get; }

        public ChannelConfig(IReadOnlyList<RouteConfig> routes = null, IReadOnlyList<ClusterConfig> clusters = null)
        {
            Routes = routes ?? new List<RouteConfig>();
            Clusters = clusters ?? new List<ClusterConfig>();
            ChangeToken = new CancellationChangeToken(_cancellationTokenSource.Token);
        }

        internal void SignalChange()
        {
            _cancellationTokenSource.Cancel();
        }

    }
}
