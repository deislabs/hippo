using Hippo.Core.Interfaces;
using Hippo.Infrastructure.ReverseProxies;
using Microsoft.Extensions.DependencyInjection;
using Yarp.ReverseProxy.Configuration;

namespace Hippo.Infrastructure.Extensions
{
    /// <summary>
    /// Extends the IReverseProxyBuilder to support the ChannelConfigProvider
    /// </summary>
    public static class ChannelConfigProviderExtensions
    {
        public static IReverseProxyBuilder LoadFromHippoChannels(this IReverseProxyBuilder builder)
        {
            builder.Services.AddSingleton<ChannelConfigProvider>();
            builder.Services.AddSingleton<IReverseProxyUpdater>(f => f.GetRequiredService<ChannelConfigProvider>());
            builder.Services.AddSingleton<IProxyConfigProvider>(f => f.GetRequiredService<ChannelConfigProvider>());
            return builder;
        }
    }
}
