using System;

namespace Hippo.Core.ReverseProxies
{
    public enum ReverseProxyAction
    {
        Start,
        Stop
    }

    public record ReverseProxyUpdateRequest(Guid ApplicationId, Guid ChannelId, string Host, string Domain, ReverseProxyAction Action);
}
