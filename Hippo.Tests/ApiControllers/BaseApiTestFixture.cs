using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using Hippo.ApiControllers;
using Hippo.Messages;
using Hippo.Models;
using Hippo.Repositories;
using Hippo.Tasks;
using Hippo.Tests.Stubs;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Hippo.Tests.ApiControllers
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
                Owner = User
            };
            Context = new DataContext(
              new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: testDatabaseName)
                .Options);
            Context.Database.EnsureDeleted();
            Context.Database.EnsureCreated();
            Context.Applications.Add(Application);
            Context.Users.Add(User);
            Context.SaveChanges();
            TokenIssuer = new MockTokenIssuer();
        }
    }
}
