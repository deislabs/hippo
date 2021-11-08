using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Hippo.Infrastructure.Config
{
    public static class ConfigurationBuilderExtensions
    {
        public static IConfigurationBuilder AddChannelConfiguration(this IConfigurationBuilder builder, ChannelConfigurationProvider channelConfigurationProvider)
        {
            return builder.Add(new ChannelConfigurationSource(channelConfigurationProvider));
        }
    }
}
