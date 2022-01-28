using Hippo.Application.Common.Exceptions;
using Hippo.Application.Common.Interfaces;
using Hippo.Core.Entities;
using Hippo.Infrastructure.ReverseProxies.Configuration;
using Microsoft.Extensions.Logging;
using Yarp.ReverseProxy.Configuration;

namespace Hippo.Infrastructure.ReverseProxies;

public class YarpReverseProxy : IReverseProxy
{
    // TODO: when the host restarts, we should re-hydrate the reverse proxy route config
    //
    // We could fix this by implementing a file-backed config provider.
    private readonly IConfigProvider _configProvider;

    public YarpReverseProxy(IConfigProvider configProvider)
    {
        _configProvider = configProvider;
    }

    public void Start(Channel c, string address)
    {
        if (c.Domain != null)
        {
            var key = GetKey(c.AppId, c.Id);

            var routeConfig = new RouteConfig()
            {
                RouteId = key,
                ClusterId = key,
                Match = new RouteMatch
                {
                    Hosts = new List<string>() { c.Domain },
                    Path = "{**catch-all}"
                }
            };

            var clusterConfig = new ClusterConfig()
            {
                ClusterId = key,
                Destinations = new Dictionary<string, DestinationConfig>(StringComparer.OrdinalIgnoreCase)
                    {
                        {
                            key, new DestinationConfig()
                            {
                                Address = "http://" + address
                            }
                        }
                    }
            };

            _configProvider.AddCluster(clusterConfig);
            _configProvider.AddRoute(routeConfig);
        }
    }

    public void Stop(Channel c)
    {
        var key = GetKey(c.AppId, c.Id);
        try
        {
            _configProvider.RemoveRoute(key);
            _configProvider.RemoveCluster(key);
        }
        // do nothing; the route/cluster has already been removed
        catch (NotFoundException) { }
    }

    private static string GetKey(Guid appId, Guid channelId) => $"App:{appId}-Channel:{channelId}";
}
