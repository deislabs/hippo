namespace Hippo.Services
{
    public interface ITraefikService
    {
        void StartProxy(string name, string hostname, string proxyUrl);
        void StopProxy(string name);
    }
}
