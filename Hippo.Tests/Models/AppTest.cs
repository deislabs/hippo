using System.Collections.Generic;
using Hippo.Models;
using Xunit;

namespace Hippo.Tests.Models
{
    public class AppTest
    {
        private readonly App One;

        public AppTest()
        {
            One = new App
            {
                Name = "one",
                Owner = new Account
                {
                    UserName = "user",
                },
                Collaborators = new List<Account>(),
                Domains = new List<Domain>(),
                Releases = new List<Release>
                {
                    new Release
                    {
                        Revision = 1,
                        Build = new Build
                        {
                            UploadUrl = "bindle:bacongobbler.com/example/1"
                        },
                        Config = new Config{
                            EnvironmentVariables = new List<EnvironmentVariable>()
                        },
                    }
                }
            };
        }

        [Fact]
        public void TestTraefikConfig()
        {
            Assert.Equal("", One.TraefikConfig());
        }
    }
}
