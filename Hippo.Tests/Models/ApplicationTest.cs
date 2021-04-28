using System.Collections.Generic;
using Hippo.Models;
using Xunit;

namespace Hippo.Tests.Models
{
    public class ApplicationTest
    {
        private readonly Application One;

        public ApplicationTest()
        {
            One = new Application
            {
                Name = "one",
                Owner = new Account
                {
                    UserName = "user",
                },
                Collaborators = new List<Account>(),
                Releases = new List<Release>
                {
                    new Release
                    {
                        Revision = "2.0.0",
                        UploadUrl = "bindle:hippos.rocks/one/2.0.0"
                    }
                },
                Channels = new List<Channel>
                {
                    new Channel
                    {
                        Name = "development",
                        Release = new Release
                        {
                            Revision = "1.0.0",
                            UploadUrl = "bindle:hippos.rocks/one/1.0.0"
                        },
                        Configuration = new Configuration
                        {
                            EnvironmentVariables = new List<EnvironmentVariable>
                            {
                                new EnvironmentVariable
                                {
                                    Key = "HELLO",
                                    Value = "world"
                                }
                            }
                        },
                    }
                }
            };
        }
    }
}
