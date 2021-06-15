using Hippo.Models;
using Hippo.Repositories;
using Hippo.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Hippo.Controllers
{
    public class AccountController : HippoController
    {
        private readonly SignInManager<Account> _signInManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;

        // TODO: assess logging code for PII/GDPR implications
        public AccountController(SignInManager<Account> signInManager, IUnitOfWork unitOfWork, IConfiguration configuration, ILogger<AccountController> logger)
            : base(logger)
        {
            this._signInManager = signInManager;
            this._unitOfWork = unitOfWork;
            this._configuration = configuration;
        }

        public IActionResult Register()
        {
            TraceMethodEntry();

            if (this.User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "App");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(AccountRegisterForm form)
        {
            TraceMethodEntry(WithArgs(form));

            if (ModelState.IsValid)
            {
                var account = new Account
                {
                    UserName = form.UserName,
                    Email = form.Email,
                };

                var result = await _signInManager.UserManager.CreateAsync(account, form.Password);
                if (result.Succeeded)
                {
                    _logger.LogTrace($"Register: created user {form.UserName}");
                    if (_unitOfWork.Accounts.IsEmpty())
                    {
                        // assign first user as Administrator
                        var roleResult = await _signInManager.UserManager.AddToRoleAsync(account, "Administrator");
                        if (!roleResult.Succeeded)
                        {
                            ModelState.AddModelError("", "failed to assign role 'Administrator'");
                            foreach (IdentityError error in result.Errors)
                            {
                                ModelState.AddModelError("", error.Description);
                            }
                        }
                        else
                        {
                            _logger.LogInformation($"Register: {form.UserName} has been granted the 'Administrator' role");
                        }
                    }
                    return RedirectToAction("Login", "Account");
                }
                else
                {
                    _logger.LogWarning($"Register: error(s) creating user {form.UserName}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
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
            TraceMethodEntry();

            if (this.User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "App");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginForm form)
        {
            TraceMethodEntry(WithArgs(form));

            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(form.UserName, form.Password, form.RememberMe, false);
                if (result.Succeeded)
                {
                    _logger.LogTrace($"Login {form.UserName}: succeeded: {SigninFailureLogMessage(result)}");

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
                    _logger.LogWarning($"Login {form.UserName}: failed: {SigninFailureLogMessage(result)}");

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
            TraceMethodEntry();

            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        private static string SigninFailureLogMessage(Microsoft.AspNetCore.Identity.SignInResult result)
        {
            var reasons = new List<string>();
            if (result.IsNotAllowed)
            {
                reasons.Add("not allowed");
            }
            if (result.IsLockedOut)
            {
                reasons.Add("locked out");
            }
            if (result.RequiresTwoFactor)
            {
                reasons.Add("needs 2FA");
            }
            return string.Join(",", reasons);
        }
    }
}
