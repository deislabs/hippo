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
                    IsSuperUser = true,
                };
            }

            var result = await userManager.CreateAsync(user, "Passw0rd!");
            if (result != IdentityResult.Success)
            {
                throw new InvalidOperationException("Failed to create default user");
            }

            if (!context.Applications.Any())
            {
                var applications = new List<App>()
                {
                    new App
                    {
                        Name = "helloworld",
                        Owner = user,
                        Releases = new List<Release>()
                        {
                            new Release
                            {
                                Revision = 1,
                                Build = new Build
                                {
                                    UploadUrl = "bindle:hippos.rocks/helloworld/1"
                                },
                                Config = new Config
                                {
                                    EnvironmentVariables = new List<EnvironmentVariable>()
                                    {
                                        new EnvironmentVariable
                                        {
                                            Key = "HELLO",
                                            Value = "world",
                                        }
                                    }
                                }
                            }
                        }
                    }
                };

                context.AddRange(applications);
                context.SaveChanges();
            }
        }
    }
}
