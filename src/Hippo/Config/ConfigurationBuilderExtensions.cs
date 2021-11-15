using Microsoft.Extensions.Configuration;

namespace Hippo.Config;

public static class ConfigurationBuilderExtensions
{
    public static IConfigurationBuilder AddChannelConfiguration(this IConfigurationBuilder builder, ChannelConfigurationProvider channelConfigurationProvider)
    {
        return builder.Add(new ChannelConfigurationSource(channelConfigurationProvider));
    }
}
