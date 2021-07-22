using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using Hippo.Controllers;
using Hippo.Models;
using Hippo.Repositories;
using Hippo.Tests.Stubs;
using Hippo.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

[assembly: System.CLSCompliant(false)]

namespace Hippo.Tests.Controllers
{
    public class AccountControllerTest
    {
        private readonly AccountController _controller;
        private readonly DataContext _context;
        private readonly Account _admin;

        private readonly Account _user;

        public AccountControllerTest()
        {
            this._admin = new Account
            {
                UserName = "admin",
                Id = "1",
            };
            this._user = new Account
            {
                UserName = "user",
                Id = "2"
            };
            _context = new InMemoryDataContext();
            var configuration = new Mock<IConfiguration>();
            _controller = new AccountController(new FakeSignInManager(new FakeUserManager(_context)), new DbUnitOfWork(_context, new FakeCurrentUser(_admin.UserName)), configuration.Object, new NullLogger<AccountController>());
            var identity = new GenericIdentity(_admin.UserName);
            var context = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(identity)
                }
            };
            _controller.ControllerContext = context;
        }

        [Fact]
        public async void TestRegister()
        {
            var viewResult = _controller.Register();
            Assert.NotNull(viewResult);

            var registerResult = await _controller.Register(new AccountRegisterForm
            {
                UserName = "test",
                Email = "test@hippos.rocks",
                Password = "foobar",
                PasswordConfirm = "foobar",
            });
            Assert.NotNull(registerResult);
            Assert.Equal(typeof(RedirectToActionResult), registerResult.GetType());
            Assert.Equal("Login", ((RedirectToActionResult)registerResult).ActionName);

            Assert.NotNull(_context.Accounts.Where(a => a.UserName == "test").SingleOrDefault());
        }
    }
}
