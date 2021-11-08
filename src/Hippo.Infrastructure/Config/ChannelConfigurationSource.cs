using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Hippo.Infrastructure.Config
{
    public class ChannelConfigurationSource : IConfigurationSource
    {
        private readonly ChannelConfigurationProvider _channelConfigurationProvider;
        public ChannelConfigurationSource(ChannelConfigurationProvider channelConfigurationProvider)
        {
            _channelConfigurationProvider = channelConfigurationProvider;
        }
        public IConfigurationProvider Build(IConfigurationBuilder builder) =>
           _channelConfigurationProvider;
    }
}
