namespace Hippo.Services
{
    public interface ITraefikService
    {
        void StartProxy(string name, string hostname, string backend);
        void StopProxy(string name);
    }
}
