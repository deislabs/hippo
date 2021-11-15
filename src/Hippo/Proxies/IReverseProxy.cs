using Hippo.Models;

namespace Hippo.Proxies;

public interface IReverseProxy
{
    void StartProxy(Channel channel, string address);
    void StopProxy(Channel channel);
}
