using Hippo.Application.Accounts.Commands;
using Hippo.Application.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Hippo.Web.Controllers;

public class AccountController : WebUIControllerBase
{
    [HttpGet]
    public ActionResult Register()
    {
        return View(new CreateAccountCommand());
    }

    [HttpPost]
    public async Task<ActionResult<string>> Register(CreateAccountCommand command)
    {
        try
        {
            await Mediator.Send(command);
            return RedirectToAction(nameof(Login));
        }
        catch (Exception e)
        {
            ModelState.AddModelError("", e.Message);
            return View(command);
        }
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View(new LoginAccountCommand());
    }

    [HttpPost]
    public async Task<ActionResult> Login(LoginAccountCommand command)
    {
        try
        {
            await Mediator.Send(command);
            return RedirectToAction("Index", "App");
        }
        catch (Exception e)
        {
            ModelState.AddModelError("", e.Message);
            return View(command);
        }
    }

    [HttpGet]
    [HttpPost]
    public async Task<IActionResult> Logout(LogoutAccountCommand command)
    {
        await Mediator.Send(command);
        return RedirectToAction(nameof(Login));
    }
}
