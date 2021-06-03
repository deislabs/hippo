using Hippo.Controllers;
using Hippo.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Threading;
using System.Security.Claims;
using System.Security.Principal;
using Xunit;
using Microsoft.EntityFrameworkCore;
using Hippo.Tests.Schedulers;
using Hippo.Repositories;
using Hippo.Tests.Stubs;

namespace Hippo.Tests.Controllers
{
    public class AppControllerTest
    {
        private readonly AppController controller;

        private readonly Account admin;

        private readonly Account user;

        public AppControllerTest()
        {
            admin = new Account
            {
                UserName = "admin",
                Id = "1",
            };
            user = new Account
            {
                UserName = "user",
                Id = "2"
            };
            var store = new Mock<IUserStore<Account>>();
            store.Setup(x => x.FindByIdAsync("1", CancellationToken.None))
            .ReturnsAsync(admin);
            store.Setup(x => x.FindByIdAsync("2", CancellationToken.None))
            .ReturnsAsync(user);
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "Hippo")
                .Options;
            var context = new DataContext(options);
            var userManager = new UserManager<Account>(store.Object, null, null, null, null, null, null, null, null);
            var jobScheduler = new FakeJobScheduler();
            controller = new AppController(new DbUnitOfWork(context, new FakeCurrentUser(admin.UserName)), userManager, jobScheduler, new NullLogger<AppController>());
        }

        [Fact]
        public void TestGetApps()
        {
            var fakeIdentity = new GenericIdentity(admin.UserName);
            var context = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(fakeIdentity)
                }
            };
            controller.ControllerContext = context;

            var viewResult = controller.Index();
            Assert.NotNull(viewResult);
        }
    }

    class FakeCurrentUser: ICurrentUser
    {
        private readonly string _name;

        public FakeCurrentUser(string name)
        {
            _name = name;
        }

        public string Name() => _name;
    }
}
