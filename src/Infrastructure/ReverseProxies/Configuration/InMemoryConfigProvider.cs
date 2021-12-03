using Hippo.Application.Common.Exceptions;
using Microsoft.Extensions.Primitives;
using Yarp.ReverseProxy.Configuration;

namespace Hippo.Infrastructure.ReverseProxies.Configuration;

/// <summary
/// This IProxyConfigProvider loads routes and clusters from memory.
/// </summary>
public class InMemoryConfigProvider : IProxyConfigProvider
{
    private List<RouteConfig> _routes = new List<RouteConfig>();

    private List<ClusterConfig> _clusters = new List<ClusterConfig>();

    private volatile InMemoryConfig _config = new InMemoryConfig(new List<RouteConfig>(), new List<ClusterConfig>());

    public IProxyConfig GetConfig() => _config;

    public void AddRoute(RouteConfig routeConfig)
    {
        _routes.Add(routeConfig);
        Update();
    }

    public void AddCluster(ClusterConfig clusterConfig)
    {
        _clusters.Add(clusterConfig);
        Update();
    }

    public void RemoveRoute(string id)
    {
        int index = _routes.FindIndex(r => r.RouteId == id);
        if (index == -1)
        {
            throw new NotFoundException(nameof(RouteConfig), id);
        }
        _routes.RemoveAt(index);
        Update();
    }

    public void RemoveCluster(string id)
    {
        int index = _clusters.FindIndex(c => c.ClusterId == id);
        if (index == -1)
        {
            throw new NotFoundException(nameof(ClusterConfig), id);
        }
        _clusters.RemoveAt(index);
        Update();
    }

    private void Update()
    {
        var oldConfig = _config;
        _config = new InMemoryConfig(_routes, _clusters);
        oldConfig.SignalChange();
    }

    private class InMemoryConfig : IProxyConfig
    {
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();

        public InMemoryConfig(IReadOnlyList<RouteConfig> routes, IReadOnlyList<ClusterConfig> clusters)
        {
            Routes = routes;
            Clusters = clusters;
            ChangeToken = new CancellationChangeToken(_cts.Token);
        }

        public IReadOnlyList<RouteConfig> Routes { get; }

        public IReadOnlyList<ClusterConfig> Clusters { get; }

        public IChangeToken ChangeToken { get; }

        internal void SignalChange()
        {
            _cts.Cancel();
        }
    }
}
