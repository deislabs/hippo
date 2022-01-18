using Yarp.ReverseProxy.Configuration;

namespace Hippo.Infrastructure.ReverseProxies.Configuration;

public interface IConfigProvider
{
    void AddCluster(ClusterConfig clusterConfig);
    void AddRoute(RouteConfig routeConfig);
    void RemoveCluster(string id);
    void RemoveRoute(string id);
}
