using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using Hippo.Controllers;
using Hippo.Models;
using Hippo.Repositories;
using Hippo.Tasks;
using Hippo.Tests.Fakes;
using Hippo.Tests.Schedulers;
using Hippo.Tests.Stubs;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace Hippo.Tests.Controllers
{
    public class AppControllerTest
    {
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
        }

        private AppController MakeController(string dbName)
        {
            var store = new Mock<IUserStore<Account>>();
            store.Setup(x => x.FindByIdAsync("1", CancellationToken.None))
            .ReturnsAsync(admin);
            store.Setup(x => x.FindByIdAsync("2", CancellationToken.None))
            .ReturnsAsync(user);
            store.Setup(x => x.FindByNameAsync("admin", CancellationToken.None))
            .ReturnsAsync(admin);
            store.Setup(x => x.FindByNameAsync("user", CancellationToken.None))
            .ReturnsAsync(user);
            var context = DbContext(dbName);
            var userManager = new UserManager<Account>(store.Object, null, null, null, null, null, null, null, null);
            var taskQueue = new FakeTaskQueue<ChannelReference>();
            var unitOfWork = new DbUnitOfWork(context, new FakeCurrentUser(admin.UserName));
            var controller = new AppController(unitOfWork, userManager, taskQueue, new NullLogger<AppController>());
            return controller;
        }

        private static DataContext DbContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            return new DataContext(options);
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
            var controller = MakeController("testgetapps");
            controller.ControllerContext = context;

            var viewResult = controller.Index();
            Assert.NotNull(viewResult);
        }

        [Fact]
        public async Task EditAppStoresAllFields()
        {
            var fakeIdentity = new GenericIdentity(admin.UserName);
            var context = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(fakeIdentity)
                }
            };

            // Simulate a request to New

            var controllerN = MakeController("editapps");
            controllerN.ControllerContext = context;

            Assert.Equal(0, DbContext("editapps").Applications.Count());
            await controllerN.New(new ViewModels.AppNewForm { Name = "foo", StorageId = "contoso/birdsondemand" });
            Assert.Equal(1, DbContext("editapps").Applications.Count());

            var appN = DbContext("editapps").Applications.Single();
            Assert.Equal("foo", appN.Name);
            Assert.Equal("contoso/birdsondemand", appN.StorageId);

            // Simulate a request to Edit

            var contextE = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(fakeIdentity)
                }
            };

            var controllerE = MakeController("editapps");
            controllerE.ControllerContext = contextE;

            var res = await controllerE.Edit(appN.Id, new ViewModels.AppEditForm { Id = appN.Id, Name = "bar", StorageId = "contoso/birdsdoingtheirownthing" });

            var appE = DbContext("editapps").Applications.Single();
            Assert.Equal("bar", appE.Name);
            Assert.Equal("contoso/birdsdoingtheirownthing", appE.StorageId);
        }
    }
}
