using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;

namespace Hippo.Models
{
    public class DataSeeder
    {
        private const string adminUserName = "admin";
        private readonly DataContext context;
        private readonly UserManager<Account> userManager;

        public DataSeeder(DataContext context, UserManager<Account> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }


        public async Task CreateUser(string userName)
        {
            var user = await userManager.FindByEmailAsync($"{userName}@hippos.rocks");
            if (user == null)
            {
                user = new Account
                {
                    UserName = userName,
                    Email = $"{userName}@hippos.rocks",
                };
                var result = await userManager.CreateAsync(user, "Passw0rd!");
                if (!result.Succeeded)
                {
                    throw new InvalidOperationException($"Failed to create user {userName}");
                }

                if (IsAdminUser(userName))
                {
                    var roleResult = await userManager.AddToRoleAsync(user, "Administrator");
                    if (!roleResult.Succeeded)
                    {
                        throw new InvalidOperationException($"Failed to assign {userName} as Administrator");
                    }
                }
            }
        }

        public bool IsAdminUser(string userName)
            => userName == adminUserName;

        public async Task<Account> GetAdminUser()
            => await userManager.FindByNameAsync(adminUserName);

        public async Task Seed()
        {
            context.Database.EnsureCreated();

            var users = new string[] { "dev", adminUserName };

            foreach (var user in users)
            {
                await CreateUser(user);
            }

            if (!context.Applications.Any())
            {
                var revisions = new List<Revision>
                {
                    new Revision { RevisionNumber = "1.0.0" },
                    new Revision { RevisionNumber = "1.1.0" },
                    new Revision { RevisionNumber = "1.2.0-rc3" },
                    new Revision { RevisionNumber = "1.2.0-rc4" },
                    new Revision { RevisionNumber = "2.0.0" },
                    // The following revisions exist in the test server, but are not seeded
                    // as registered so they can be used for rulesy channel upgrade testing:
                    // 1.1.1, 1.1.3, 1.3.0, 2.0.1, 2.0.2, 2.1.0
                    //
                    // (Don't use 1.1.2. I messed it up.)
                };

                var applications = new List<Application>
                {
                    new Application
                    {
                        Name = "helloworld",
                        Owner = await GetAdminUser(),
                        StorageId = "hippos.rocks/helloworld",
                        Revisions = revisions,
                    }
                };

                var application = applications[0];

                application.Channels = new List<Channel>
                {
                    new Channel
                    {
                        Name = "Development",
                        Application = application,
                        PortID = 0,
                        RevisionSelectionStrategy = ChannelRevisionSelectionStrategy.UseSpecifiedRevision,
                        SpecifiedRevision = revisions[1],
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
                        Domain = new Domain
                        {
                            Name = "app.hippos.rocks"
                        }
                    },
                    new Channel
                    {
                        Name = "Staging",
                        Application = application,
                        PortID = 1,
                        RevisionSelectionStrategy = ChannelRevisionSelectionStrategy.UseSpecifiedRevision,
                        SpecifiedRevision = revisions[0],
                        Configuration = new Configuration
                        {
                            EnvironmentVariables = new List<EnvironmentVariable>(),
                        },
                        Domain = new Domain
                        {
                            Name = "staging.hippos.rocks"
                        }
                    },
                    new Channel
                    {
                        Name = "Compatibility 1.1",
                        Application = application,
                        PortID = 2,
                        RevisionSelectionStrategy = ChannelRevisionSelectionStrategy.UseRangeRule,
                        RangeRule = "~1.1",
                        Configuration = new Configuration
                        {
                            EnvironmentVariables = new List<EnvironmentVariable>(),
                        },
                        Domain = new Domain
                        {
                            Name = "v1.hippos.rocks"
                        }
                    }
                };

                foreach (var app in applications)
                {
                    app.ReevaluateActiveRevisions();
                }

                context.Applications.AddRange(applications);
            }

            context.SaveChanges();
        }
    }
}
