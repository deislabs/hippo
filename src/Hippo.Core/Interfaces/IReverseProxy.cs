using Hippo.Core.Models;

namespace Hippo.Core.Interfaces
{
    public interface IReverseProxy
    {
        void StartProxy(Channel channel, string address);
        void StopProxy(Channel channel);
    }
}
