
using System.Collections.Generic;
using Hippo.Models;

namespace Hippo.Config
{
    public interface IChannelConfigurationProvider
    {
        public void SetBindleServer(string bindleServer);
        public void AddChannel(Channel channel, string listenAddress);
        public void RemoveChannel(Channel channel);
    }
}
