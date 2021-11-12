using Hippo.Tasks;

namespace Hippo.Proxies
{
    public interface IReverseProxyUpdater
    {
        bool UpdateProxyRecord(ReverseProxyUpdateRequest record);
        void UpdateConfig();
    }
}
