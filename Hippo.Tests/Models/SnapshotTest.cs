using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Hippo.Models;
using Hippo.Schedulers;
using Xunit;

namespace Hippo.Tests.Models
{
    public class SnapshotTest
    {
        private readonly Channel channel;

        public SnapshotTest()
        {
            channel = new Channel
            {
                Name = "development",
                Domain = new Domain
                {
                    Name = "hippos.rocks"
                },
                Configuration = new Configuration
                {
                    EnvironmentVariables = new List<EnvironmentVariable>()
                },
                Release = new Release
                {
                    Revision = "1.0.0",
                    UploadUrl = "bindle:hippos.rocks/one/1.0.0"
                }
            };
        }

        [Fact]
        public void TestCanTakeSnapshot()
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<Channel, Snapshot>());
            var mapper = new Mapper(config);
            Snapshot snapshot = mapper.Map<Snapshot>(channel);
            Assert.NotNull(snapshot);
            channel.Name = "production";
            Assert.Equal("development", snapshot.Name);
        }
    }
}
