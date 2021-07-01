using Hippo.Messages;

namespace Hippo.Providers
{
    public interface IProxyConfigUpdater
    {
        public void UpdateConfig(YarpConfigurationRequest request);
    }
}
