using System.Security.Claims;
using System.Security.Principal;
using Hippo.Controllers;
using Hippo.FunctionalTests.Fakes;
using Hippo.Models;
using Hippo.Repositories;
using Hippo.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace Hippo.FunctionalTests.Controllers;

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
            Id = "1",
            UserName = "admin"
        };

        this._user = new Account
        {
            Id = "2",
            UserName = "user"
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
            Email = "test@hippofactory.io",
            Password = "foobar",
            PasswordConfirm = "foobar"
        });
        Assert.NotNull(registerResult);
        Assert.Equal(typeof(RedirectToActionResult), registerResult.GetType());
        Assert.Equal("Login", ((RedirectToActionResult)registerResult).ActionName);
    }
}
