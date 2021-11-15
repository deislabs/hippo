using Microsoft.Extensions.Configuration;

namespace Hippo.Config;

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
