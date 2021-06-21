namespace Hippo.Schedulers
{
    public interface IPortIsInUseChecker
    {
        bool CheckPortIsInUse(int port);
    }
}
