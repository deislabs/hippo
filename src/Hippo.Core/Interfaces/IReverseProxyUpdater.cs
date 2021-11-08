using Hippo.Core.ReverseProxies;

namespace Hippo.Core.Interfaces
{
    public interface IReverseProxyUpdater
    {
        bool UpdateProxyRecord(ReverseProxyUpdateRequest record);
        void UpdateConfig();
    }
}
