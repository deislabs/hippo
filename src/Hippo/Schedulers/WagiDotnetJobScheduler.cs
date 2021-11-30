using Hippo.Config;
using Hippo.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Hippo.Schedulers;

public class WagiDotnetJobScheduler : JobScheduler
{
    private readonly IChannelConfigurationProvider _channelConfigurationProvider;
    public WagiDotnetJobScheduler(ILogger<WagiDotnetJobScheduler> logger, IChannelConfigurationProvider channelConfigurationProvider, IHostEnvironment env)
        : base(logger, env)
    {
        _channelConfigurationProvider = channelConfigurationProvider;
        _channelConfigurationProvider.SetBindleServer(_bindleUrl);
    }

    public override void Start(Channel c)
    {
        var port = c.PortID + Channel.EphemeralPortRange;
        var listenAddress = $"http://127.0.0.1:{port}";
        _channelConfigurationProvider.AddChannel(c, listenAddress);
        var data = new ChannelStartedEventArgs();
        data.Channel = c;
        data.ListenAddress = listenAddress;
        OnChannelStarted(data);
    }

    public override void Stop(Channel c)
    {
        OnChannelStopped(c);
        _channelConfigurationProvider.RemoveChannel(c);

    }
}
