using System.Collections.Generic;
using Hippo.Models;
using Xunit;

namespace Hippo.Tests.Models
{
    public class ReleaseTest
    {
        private readonly Release V1;

        public ReleaseTest()
        {
            V1 = new Release{
                Revision = "1.0.0",
                Build = new Build
                {
                    UploadUrl = "bindle:bacongobbler.com/example/1.0.0"
                },
                Config = new Config{
                    EnvironmentVariables = new List<EnvironmentVariable>()
                },
            };
        }

        [Fact]
        public void TestWagiConfig()
        {
            Assert.Equal(
@"[[module]]
module = ""bindle:bacongobbler.com/example/1.0.0""
",
            V1.WagiConfig());
        }
    }
}
