using System.Threading;
using Hippo.Core.Interfaces;
using Hippo.Core.Models;
using Hippo.Core.ReverseProxies;

namespace Hippo.Infrastructure.ReverseProxies
{
    public class YarpReverseProxy : IReverseProxy
    {
        private readonly ITaskQueue<ReverseProxyUpdateRequest> _reverseProxyConfigQueue;
        public YarpReverseProxy(ITaskQueue<ReverseProxyUpdateRequest> reverseProxyConfigQueue)
        {
            _reverseProxyConfigQueue = reverseProxyConfigQueue;
        }

        public void StopProxy(Channel channel)
        {
            _reverseProxyConfigQueue.Enqueue(new ReverseProxyUpdateRequest(channel.Application.Id, channel.Id, null, channel.Domain.Name, ReverseProxyAction.Stop), CancellationToken.None).Wait();
        }

        public void StartProxy(Channel channel, string address)
        {
            _reverseProxyConfigQueue.Enqueue(new ReverseProxyUpdateRequest(channel.Application.Id, channel.Id, address, channel.Domain.Name, ReverseProxyAction.Start), CancellationToken.None).Wait();
        }
    }
}
