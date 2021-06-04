using System.Collections.Generic;
using System.Linq;
using Hippo.Models;
using Hippo.Schedulers;
using Xunit;

namespace Hippo.Tests.Schedulers
{
    public class SystemdJobSchedulerTest
    {
        private readonly Application application;

        public SystemdJobSchedulerTest()
        {
            application = new Application
            {
                Name = "one",
                StorageId = "hippos.rocks/one",
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
                        RevisionSelectionStrategy = ChannelRevisionSelectionStrategy.UseSpecifiedRevision,
                        SpecifiedRevision = new Revision
                        {
                            RevisionNumber = "1.0.0",
                        },
                        ActiveRevision = new Revision
                        {
                            RevisionNumber = "1.0.0",
                        }

                    }
                }
            };
            application.Channels.First().Application = application;
        }

        [Fact]
        public void TestWagiConfig()
        {
            Assert.Equal(
@"[[module]]
module = ""bindle:hippos.rocks/one/1.0.0""
".Trim(),
            SystemdJobScheduler.WagiConfig(application.Channels.First()).Trim());
        }

        [Fact]
        public void TestTraefikConfig()
        {
            Assert.Equal(
@"[http]

[http.routers]

[http.routers.to-one-development]
rule = ""Host(`hippos.rocks`) && PathPrefix(`/`)""
service = ""one-development""

[http.services]

[http.services.one-development]

[http.services.one-development.loadBalancer]

[[http.services.one-development.loadBalancer.servers]]
url = ""http://localhost:32768""
".Trim(),
                SystemdJobScheduler.TraefikConfig(application.Channels.First()).Trim());
        }
    }
}
