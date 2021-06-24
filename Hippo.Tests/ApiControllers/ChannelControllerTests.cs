using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.TestHost;
using Moq;
using Xunit;

namespace Hippo.Tests.ApiControllers
{
    public class ChannelControllerTestFixture : BaseApiTestFixture
    {
        const string TestDatabaseName = "HippoChannelApiControllerTest";
        public CreateChannelRequest CreateChannelRequestFixed { get; private set; }
        public CreateChannelRequest CreateChannelRequestRange { get; private set; }
        public CreateChannelRequest CreateChannelRequestInvalidApp { get; private set; }
        public readonly Mock<ITaskQueue<ChannelReference>> MockTaskQueue;

        public ChannelControllerTestFixture() : base(TestDatabaseName)
        {
            CreateChannelRequestFixed = new()
            {
                AppId = AppId,
                Name = "Test Channel with Fixed Revision",
                RevisionSelectionStrategy = ChannelRevisionSelectionStrategy.UseSpecifiedRevision,
                RevisionNumber = "1.2.3"
            };

            CreateChannelRequestInvalidApp = new()
            {
                AppId = Guid.NewGuid(),
                Name = "Test Channel with Invalid App",
                RevisionSelectionStrategy = ChannelRevisionSelectionStrategy.UseSpecifiedRevision,
                RevisionNumber = "1.2.3"
            };

            CreateChannelRequestRange = new()
            {
                AppId = AppId,
                Name = "Test Channel with Range Revision",
                RevisionSelectionStrategy = ChannelRevisionSelectionStrategy.UseRangeRule,
                RevisionRange = "~1.2.3"
            };
            MockTaskQueue = new Mock<ITaskQueue<ChannelReference>>();
            MockTaskQueue.Setup(tq => tq.Enqueue(It.IsAny<ChannelReference>(), It.IsAny<CancellationToken>())).Verifiable();
            Server = new TestServer(
              new WebHostBuilder().
                UseStartup<TestStartup>(
                context => new TestStartup(TokenIssuer, TestDatabaseName, MockTaskQueue.Object)
              )
            );
        }
    }

    public class ChannelControllerTest : IClassFixture<ChannelControllerTestFixture>
    {
        private readonly ChannelControllerTestFixture _fixture;

        public ChannelControllerTest(ChannelControllerTestFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task RequiresAuthorization()
        {
            var client = _fixture.Server.CreateClient();
            var response = await client.PostAsJsonAsync<CreateChannelRequest>("/api/channel", _fixture.CreateChannelRequestFixed);
            Assert.True(response.StatusCode == System.Net.HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task PostChannelSucceeds()
        {
            var token = _fixture.TokenIssuer.GetToken();
            var client = _fixture.Server.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Create a channel with a fixed revsion
            var response = await client.PostAsJsonAsync<CreateChannelRequest>("/api/channel", _fixture.CreateChannelRequestFixed);
            await CheckResultAsync(_fixture.CreateChannelRequestFixed, response);

            // Create a channel with a revsion range
            response = await client.PostAsJsonAsync<CreateChannelRequest>("/api/channel", _fixture.CreateChannelRequestRange);
            await CheckResultAsync(_fixture.CreateChannelRequestRange, response, true);
        }

        [Fact]
        public async Task CreateChannelSucceeds()
        {
            var controller = GetController();

            // Create a channel with a fixed revsion
            var response = await controller.New(_fixture.CreateChannelRequestFixed);
            CheckResult(_fixture.CreateChannelRequestFixed, response);

            // Create a channel with a revsion range
            response = await controller.New(_fixture.CreateChannelRequestRange);
            CheckResult(_fixture.CreateChannelRequestRange, response, true);
        }

        [Fact]
        public async Task InvalidApplicationCausesNotFoundError()
        {
            var controller = GetController();
            var response = await controller.New(_fixture.CreateChannelRequestInvalidApp);
            Assert.NotNull(response);
            Assert.IsType<NotFoundResult>(response.Result);
        }

        [Fact]
        public async Task InvalidModelCausesBadRequesstError()
        {
            var controller = GetController();
            controller.ModelState.AddModelError("Test", "TestError");
            var response = await controller.New(_fixture.CreateChannelRequestInvalidApp);
            Assert.NotNull(response);
            Assert.IsType<BadRequestObjectResult>(response.Result);
        }

        [Fact]
        public void InvalidModelCausesError()
        {
            var requests = new CreateChannelRequest[]
            {
                new()
                {
                  Name = "AppID is Missing",
                  RevisionSelectionStrategy = ChannelRevisionSelectionStrategy.UseSpecifiedRevision,
                  RevisionNumber = "1.2.3"
                },
                new()
                {
                  AppId = Guid.NewGuid(),
                  RevisionSelectionStrategy = ChannelRevisionSelectionStrategy.UseSpecifiedRevision,
                  RevisionNumber = "1.2.3"
                },
                new()
                {
                  AppId = Guid.NewGuid(),
                  Name = "Revision Strategy is Missing",
                  RevisionNumber = "1.2.3"
                },
                new()
                {
                  AppId = Guid.NewGuid(),
                  Name = "Revision Range is Missing",
                  RevisionNumber = "1.2.3",
                  RevisionSelectionStrategy = ChannelRevisionSelectionStrategy.UseRangeRule,
                },
                new()
                {
                  AppId = Guid.NewGuid(),
                  Name = "Revision Version is Missing",
                  RevisionRange = "~1.2.3",
                  RevisionSelectionStrategy = ChannelRevisionSelectionStrategy.UseSpecifiedRevision,
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

        private void CheckResult(CreateChannelRequest request, ActionResult<CreateChannelResponse> response, bool range = false)
        {
            Assert.NotNull(response);
            Assert.IsType<CreatedResult>(response.Result);
            var createdResult = response.Result as CreatedResult;
            Assert.IsType<CreateChannelResponse>(createdResult.Value);
            var result = createdResult.Value as CreateChannelResponse;
            CheckResult(request, result, range);
        }

        private async Task CheckResultAsync(CreateChannelRequest request, HttpResponseMessage response, bool range = false)
        {
            Assert.True(response.IsSuccessStatusCode);
            var createChannelResponse = await response.Content.ReadFromJsonAsync<CreateChannelResponse>();
            CheckResult(request, createChannelResponse, range);
        }

        private void CheckResult(CreateChannelRequest request, CreateChannelResponse result, bool range = false)
        {
            Assert.NotEqual(Guid.Empty, result.Id);
            Assert.Equal(request.Name, result.Name);
            if (range)
            {
                Assert.Equal(ChannelRevisionSelectionStrategy.UseRangeRule, result.RevisionSelectionStrategy);
                Assert.Equal(request.RevisionRange, result.RevisionRange);
                Assert.True(string.IsNullOrEmpty(result.RevisionNumber));
            }
            else
            {
                Assert.Equal(ChannelRevisionSelectionStrategy.UseSpecifiedRevision, result.RevisionSelectionStrategy);
                Assert.Equal(request.RevisionNumber, result.RevisionNumber);
                Assert.True(string.IsNullOrEmpty(result.RevisionRange));
            }

            _fixture.MockTaskQueue.Verify(tq => tq.Enqueue(It.Is<ChannelReference>(cr => cr.ApplicationId == request.AppId && cr.ChannelId == result.Id), It.IsAny<CancellationToken>()), Times.Once);
        }

        private ChannelController GetController()
            => new(new DbUnitOfWork(_fixture.Context, new FakeCurrentUser(_fixture.User.UserName)), _fixture.MockTaskQueue.Object, new NullLogger<ChannelController>())
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
