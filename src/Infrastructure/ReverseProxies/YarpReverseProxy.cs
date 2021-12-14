using Hippo.Application.Common.Interfaces;
using Hippo.Core.Entities;
using Hippo.Infrastructure.ReverseProxies.Configuration;
using Yarp.ReverseProxy.Configuration;

namespace Hippo.Infrastructure.ReverseProxies;

public class YarpReverseProxy : IReverseProxy
{
    private readonly InMemoryConfigProvider _configProvider;

    public YarpReverseProxy(InMemoryConfigProvider configProvider)
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
                    Hosts = new List<string>()
                        {
                            c.Domain
                        }
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
                                Address = address
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
        _configProvider.RemoveRoute(key);
        _configProvider.RemoveCluster(key);
    }

    private static string GetKey(Guid appId, Guid channelId) => $"App:{appId}-Channel:{channelId}";
}
