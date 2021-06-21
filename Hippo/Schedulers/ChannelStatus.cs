namespace Hippo.Schedulers
{
    public class ChannelStatus
    {
        public readonly bool IsRunning;
        public readonly int Port;
        public ChannelStatus(bool isRunning, int port)
        {
            IsRunning = isRunning;
            Port = port;
        }
    }
}
