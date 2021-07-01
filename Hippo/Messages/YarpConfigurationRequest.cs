using System;

namespace Hippo.Messages
{
#pragma warning disable CS1591
    public enum YarpConfigurationAction
    {
        Start,
        Stop
    }

    public class YarpConfigurationRequest
    {
        public Guid ChannelId { get; set; }
        public Guid AppId { get; set; }
        public string Hostname { get; set; }
        public string Backend { get; set; }
        public YarpConfigurationAction Action { get; set; }
    }
}
#pragma warning restore CS1591
