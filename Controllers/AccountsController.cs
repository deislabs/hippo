using hippo.Models;
using hippo.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace hippo.Controllers
{
    public class AccountsController : Controller
    {
        private readonly SignInManager<Account> signInManager;
        private readonly UserManager<Account> userManager;
        private readonly IConfiguration config;

        public AccountsController(SignInManager<Account> signInManager, UserManager<Account> userManager, IConfiguration config)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.config = config;
        }

        public IActionResult Register()
        {
            if (this.User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Apps");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(AccountRegisterForm form)
        {
            if (ModelState.IsValid)
            {
                var result = await signInManager.UserManager.CreateAsync(new Account(form.Username, form.Email), form.Password);
                if (result.Succeeded)
                {
                    // TODO: make the first user a superuser account
                    return RedirectToAction("Login", "Accounts");
                }
                else
                {
                    ModelState.AddModelError("", "failed to create account");
                    foreach (IdentityError error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            else
            {
                ModelState.AddModelError("", "failed to register");
            }
            return View();
        }

        public IActionResult Login()
        {
            if (this.User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Apps");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginForm form)
        {
            if (ModelState.IsValid)
            {
                // do stuff
                var result = await signInManager.PasswordSignInAsync(form.Username, form.Password, form.RememberMe, false);
                if (result.Succeeded)
                {
                    if (Request.Query.Keys.Contains("ReturnUrl"))
                    {
                        Redirect(Request.Query["ReturnUrl"].First());
                    }
                    else
                    {
                        return RedirectToAction("Index", "Apps");
                    }
                }
                else
                {
                    if (result.IsNotAllowed)
                    {
                        ModelState.AddModelError("", "cannot log in at this time; please contact the administrator");
                    }

                    if (result.IsLockedOut)
                    {
                        ModelState.AddModelError("", "account locked; please contact the administrator");
                    }
                }
            }
            else
            {
                ModelState.AddModelError("", "failed to login");
            }
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
