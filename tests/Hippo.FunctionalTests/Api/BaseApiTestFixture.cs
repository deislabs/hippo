using System;
using System.Collections.Generic;
using Hippo.Core.Models;
using Hippo.Infrastructure.Data;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;

namespace Hippo.FunctionalTests.ApiControllers
{
    public class BaseApiTestFixture
    {
        public MockTokenIssuer TokenIssuer { get; private set; }
        public TestServer Server { get; protected set; }
        public DataContext Context { get; protected set; }
        public Guid AppId { get; private set; }
        public Application Application { get; private set; }
        public Account User { get; private set; }

        public BaseApiTestFixture(string testDatabaseName)
        {
            AppId = Guid.NewGuid();
            User = new()
            {
                UserName = "user",
                Id = "2",
            };
            Application = new()
            {
                Id = AppId,
                Name = "Test Application",
                StorageId = "hippo/test",
                Owner = User,
                Revisions = new List<Revision> {
                    new Revision { RevisionNumber = "1.2.3" },
                },
            };
            Context = new InMemoryDataContext(testDatabaseName);
            Context.Database.EnsureDeleted();
            Context.Database.EnsureCreated();
            Context.Applications.Add(Application);
            Context.Users.Add(User);
            Context.SaveChanges();
            TokenIssuer = new MockTokenIssuer();
        }
    }
}
