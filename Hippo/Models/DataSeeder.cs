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
        private readonly DataContext context;
        private readonly UserManager<Account> userManager;

        public DataSeeder(DataContext context, UserManager<Account> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }

        public async Task Seed()
        {
            context.Database.EnsureCreated();

            var user = await userManager.FindByEmailAsync("admin@hippos.rocks");
            if (user == null)
            {
                user = new Account
                {
                    UserName = "admin",
                    Email = "admin@hippos.rocks",
                };
            }

            var result = await userManager.CreateAsync(user, "Passw0rd!");
            if (!result.Succeeded)
            {
                throw new InvalidOperationException("Failed to create default user");
            }
            var roleResult = await userManager.AddToRoleAsync(user, "Administrator");
            if (!roleResult.Succeeded)
            {
                throw new InvalidOperationException("Failed to assign default user as Administrator");
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
                };

                var applications = new List<Application>
                {
                    new Application
                    {
                        Name = "helloworld",
                        Owner = user,
                        StorageId = "hippos.rocks/helloworld",
                        Revisions = revisions,
                        Channels = new List<Channel>
                        {
                            new Channel
                            {
                                Name = "development",
                                RevisionSelectionStrategy = ChannelRevisionSelectionStrategy.UseSpecifiedRevision,
                                SpecifiedRevision = revisions[1],
                                ActiveRevision = revisions[1],
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
                            }
                        }
                    }
                };

                context.Applications.AddRange(applications);
            }

            context.SaveChanges();
        }
    }
}
