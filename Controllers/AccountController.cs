using Hippo.Models;
using Hippo.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hippo.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<Account> signInManager;
        private readonly UserManager<Account> userManager;
        private readonly DataContext context;

        public AccountController(SignInManager<Account> signInManager, UserManager<Account> userManager, DataContext context)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.context = context;
        }

        public IActionResult Register()
        {
            if (this.User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "App");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(AccountRegisterForm form)
        {
            if (ModelState.IsValid)
            {
                var account = new Account
                {
                    UserName = form.UserName,
                    Email = form.Email,
                };
                if (context.Accounts.Count() == 0) {
                    // first account is a super user
                    account.IsSuperUser = true;
                }

                var result = await signInManager.UserManager.CreateAsync(account, form.Password);
                if (result.Succeeded)
                {
                    return RedirectToAction("Login", "Account");
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
                return RedirectToAction("Index", "App");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginForm form)
        {
            if (ModelState.IsValid)
            {
                var result = await signInManager.PasswordSignInAsync(form.Username, form.Password, form.RememberMe, false);
                if (result.Succeeded)
                {
                    if (Request.Query.Keys.Contains("ReturnUrl"))
                    {
                        Redirect(Request.Query["ReturnUrl"].First());
                    }
                    else
                    {
                        return RedirectToAction("Index", "App");
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
