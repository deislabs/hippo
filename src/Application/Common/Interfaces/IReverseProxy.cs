using Hippo.Core.Entities;

namespace Hippo.Application.Common.Interfaces;

public interface IReverseProxy
{
    void Start(Channel c, string address);

    void Stop(Channel c);
}

public class ChannelStatus
{
    public readonly bool IsRunning;
    public readonly string ListenAddress;

    public ChannelStatus(bool isRunning, string listenAddress)
    {
        IsRunning = isRunning;
        ListenAddress = listenAddress;
    }
}
