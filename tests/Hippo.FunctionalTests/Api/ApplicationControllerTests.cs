using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using Hippo.Core.Messages;
using Hippo.Core.Models;
using Hippo.Core.Tasks;
using Hippo.FunctionalTests.Fakes;
using Hippo.FunctionalTests.Stubs;
using Hippo.Infrastructure.Data;
using Hippo.Infrastructure.Services;
using Hippo.Web.Api;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.TestHost;
using Moq;
using Xunit;

namespace Hippo.FunctionalTests.ApiControllers
{
    public class ApplicationControllerTestFixture : BaseApiTestFixture
    {
        const string TestDatabaseName = "HippoApplicationApiControllerTest";
        public CreateApplicationRequest CreateApplicationRequest = new()
        {
            ApplicationName = "Test Application",
            StorageId = "hippo/test"
        };
        public readonly UserManager<Account> UserManager;

        public ApplicationControllerTestFixture() : base(TestDatabaseName)
        {
            var store = new Mock<IUserStore<Account>>();
            store
              .Setup(x => x.FindByIdAsync("2", CancellationToken.None))
              .ReturnsAsync(User);
            Context = new InMemoryDataContext(TestDatabaseName);
            UserManager = new UserManager<Account>(store.Object, null, null, null, null, null, null, null, null);
            Server = new TestServer(
              new WebHostBuilder().
                UseStartup<TestStartup>(
                context => new TestStartup(TokenIssuer, TestDatabaseName, new FakeTaskQueue<ChannelReference>())
              )
            );
        }
    }

    public class ApplicationControllerTest : IClassFixture<ApplicationControllerTestFixture>
    {
        private readonly ApplicationControllerTestFixture _fixture;
        public ApplicationControllerTest(ApplicationControllerTestFixture fixture)
        {
            this._fixture = fixture;
        }

        [Fact]
        public async Task RequiresAuthorization()
        {
            var client = _fixture.Server.CreateClient();
            var response = await client.PostAsJsonAsync<CreateApplicationRequest>("/api/application", _fixture.CreateApplicationRequest);
            Assert.True(response.StatusCode == System.Net.HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task PostApplicationSucceeds()
        {
            var token = _fixture.TokenIssuer.GetToken();
            var client = _fixture.Server.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.PostAsJsonAsync<CreateApplicationRequest>("/api/application", _fixture.CreateApplicationRequest);
            Assert.True(response.IsSuccessStatusCode);
            var createApplicationResponse = await response.Content.ReadFromJsonAsync<CreateApplicationResponse>();
            Assert.NotEqual(createApplicationResponse.Id, Guid.Empty);
            Assert.Equal(_fixture.CreateApplicationRequest.ApplicationName, createApplicationResponse.ApplicationName);
            Assert.Equal(_fixture.CreateApplicationRequest.StorageId, createApplicationResponse.StorageId);
        }

        [Fact]
        public async Task CreateApplicationSucceeds()
        {
            var controller = new ApplicationController(new DbUnitOfWork(_fixture.Context, new FakeCurrentUser(_fixture.User.UserName)), _fixture.UserManager, new FakeTaskQueue<ChannelReference>(), new NullLogger<ApplicationController>())
            {
                ControllerContext = new()
                {
                    HttpContext = new DefaultHttpContext
                    {
                        User = new ClaimsPrincipal(new GenericIdentity(_fixture.User.UserName))
                    }
                }
            };
            var response = await controller.New(_fixture.CreateApplicationRequest);
            Assert.NotNull(response);
            Assert.IsType<CreatedResult>(response.Result);
            var createdResult = response.Result as CreatedResult;
            Assert.IsType<CreateApplicationResponse>(createdResult.Value);
            var result = createdResult.Value as CreateApplicationResponse;
            Assert.NotEqual(result.Id, Guid.Empty);
            Assert.Equal(_fixture.CreateApplicationRequest.ApplicationName, result.ApplicationName);
            Assert.Equal(_fixture.CreateApplicationRequest.StorageId, result.StorageId);
        }

        [Fact]
        public async Task InvalidModelCausesBadRequesstError()
        {
            var controller = GetController();
            controller.ModelState.AddModelError("Test", "TestError");
            var response = await controller.New(_fixture.CreateApplicationRequest);
            Assert.NotNull(response);
            Assert.IsType<BadRequestObjectResult>(response.Result);
        }

        [Fact]
        public void InvalidModelCausesError()
        {
            var requests = new CreateApplicationRequest[]
            {
                new()
                {
                    ApplicationName = "Storage Id is missing"
                },
                new()
                {
                    StorageId = "hippo/test"
                }
            };

            foreach (var request in requests)
            {
                var context = new ValidationContext(request);
                var validationResults = new List<ValidationResult>();
                var result = Validator.TryValidateObject(request, context, validationResults, true);
                Assert.False(result);
            }
        }

        private ApplicationController GetController()
            => new(new DbUnitOfWork(_fixture.Context, new FakeCurrentUser(_fixture.User.UserName)), _fixture.UserManager, new FakeTaskQueue<ChannelReference>(), new NullLogger<ApplicationController>())
            {
                ControllerContext = new()
                {
                    HttpContext = new DefaultHttpContext
                    {
                        User = new ClaimsPrincipal(new GenericIdentity(_fixture.User.UserName))
                    }
                }
            };
    }
}
