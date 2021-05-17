using System.Collections.Generic;
using System.Linq;
using Hippo.Models;
using Xunit;

namespace Hippo.Tests.Models
{
    public class ChannelTest
    {
        private readonly Application application;

        public ChannelTest()
        {
            application = new Application
            {
                Name = "one",
                Channels = new List<Channel>
                {
                    new Channel
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
                    }
                }
            };
        }

        [Fact]
        public void TestWagiConfig()
        {
            Assert.Equal(
@"[[module]]
module = ""bindle:hippos.rocks/one/1.0.0""
",
            application.Channels.First().WagiConfig());
        }
    }
}
