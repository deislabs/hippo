using System;
using System.Threading;
using Hippo.Models;
using Hippo.Schedulers;
using Hippo.Tasks;

namespace Hippo.Proxies;

public class YarpReverseProxy : IReverseProxy
{
    private readonly ITaskQueue<ReverseProxyUpdateRequest> _reverseProxyConfigQueue;
    public YarpReverseProxy(ITaskQueue<ReverseProxyUpdateRequest> reverseProxyConfigQueue, JobScheduler scheduler)
    {
        _reverseProxyConfigQueue = reverseProxyConfigQueue;
        JobScheduler s = scheduler as JobScheduler;
        if (s != null)
        {
            SubscribeToJobScheduler(s);
        }
    }

    private void SubscribeToJobScheduler(JobScheduler scheduler)
    {
        scheduler.ChannelStarted += new EventHandler<ChannelStartedEventArgs>((o, e) => StartProxy(e.Channel, e.ListenAddress));
        scheduler.ChannelStopped += new EventHandler<Channel>((o, e) => StopProxy(e));
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
