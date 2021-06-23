using System;
using System.IdentityModel.Tokens.Jwt;
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
    public class ApplicationControllerTestFixture
    {
        public MockTokenIssuer TokenIssuer { get; private set; }

        public TestServer Server { get; private set; }

        public CreateApplicationRequest CreateApplicationRequest = new()
        {
            ApplicationName = "Test Application",
            StorageId = "hippo/test"
        };

        public readonly DataContext Context;
        public readonly UserManager<Account> UserManager;

        public Account User = new()
        {
            UserName = "user",
            Id = "2"
        };

        public ApplicationControllerTestFixture()
        {
            TokenIssuer = new MockTokenIssuer();
            Server = new TestServer(
              new WebHostBuilder().
                UseStartup<TestStartup>(
                context => new TestStartup(TokenIssuer)
              )
            );

            var store = new Mock<IUserStore<Account>>();
            store
              .Setup(x => x.FindByIdAsync("2", CancellationToken.None))
              .ReturnsAsync(User);
            Context = new DataContext(
              new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "Hippo")
                .Options);
            UserManager = new UserManager<Account>(store.Object, null, null, null, null, null, null, null, null);
        }
    }

    public class ApplicationControllerTest : IClassFixture<ApplicationControllerTestFixture>
    {
        private readonly ApplicationControllerTestFixture fixture;

        public ApplicationControllerTest(ApplicationControllerTestFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public async Task RequiresAuthorization()
        {
            var client = fixture.Server.CreateClient();
            var response = await client.PostAsJsonAsync<CreateApplicationRequest>("/api/application", fixture.CreateApplicationRequest);
            Assert.True(response.StatusCode == System.Net.HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task PostApplication()
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, "testuser@test.com"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, "testuser")
            };
            var token = fixture.TokenIssuer.GetToken(claims);
            var client = fixture.Server.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.PostAsJsonAsync<CreateApplicationRequest>("/api/application", fixture.CreateApplicationRequest);
            Assert.True(response.IsSuccessStatusCode);
            var createApplicationResponse = await response.Content.ReadFromJsonAsync<CreateApplicationResponse>();
            Assert.NotEqual(createApplicationResponse.Id, Guid.Empty);
            Assert.Equal(createApplicationResponse.ApplicationName, fixture.CreateApplicationRequest.ApplicationName);
            Assert.Equal(createApplicationResponse.StorageId, fixture.CreateApplicationRequest.StorageId);
        }

        [Fact]
        public async Task CreateApplication()
        {
            var controller = new ApplicationController(new DbUnitOfWork(fixture.Context, new FakeCurrentUser(fixture.User.UserName)), fixture.UserManager, new NullLogger<ApplicationController>())
            {
                ControllerContext = new()
                {
                    HttpContext = new DefaultHttpContext
                    {
                        User = new ClaimsPrincipal(new GenericIdentity(fixture.User.UserName))
                    }
                }
            };
            var response = await controller.New(fixture.CreateApplicationRequest);
            Assert.NotNull(response);
            Assert.IsType<CreatedResult>(response.Result);
            var createdResult = response.Result as CreatedResult;
            Assert.IsType<CreateApplicationResponse>(createdResult.Value);
            var result = createdResult.Value as CreateApplicationResponse;
            Assert.NotEqual(result.Id, Guid.Empty);
            Assert.Equal(result.ApplicationName, fixture.CreateApplicationRequest.ApplicationName);
            Assert.Equal(result.StorageId, fixture.CreateApplicationRequest.StorageId);
        }
    }
}
