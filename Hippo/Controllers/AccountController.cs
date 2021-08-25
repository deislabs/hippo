using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Hippo.ControllerCore;
using Hippo.Extensions;
using Hippo.Models;
using Hippo.Repositories;
using Hippo.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

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

        public async Task<IActionResult> Register()
        {
            TraceMethodEntry();

            if (this.User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "App");
            }
            var form = new AccountRegisterForm
            {
                AuthenticationSchemes = await HttpContext.GetExternalProvidersAsync(),
            };
            return View(form);
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
                    return RedirectToAction(nameof(Login));
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

        /// <summary>
        /// Register an account using the given external login provider.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> ExternalRegister([FromForm] string provider)
        {
            TraceMethodEntry();

            // Note: the "provider" parameter corresponds to the external
            // authentication provider chosen by the user agent.
            if (string.IsNullOrWhiteSpace(provider))
            {
                return BadRequest();
            }

            if (!await HttpContext.IsProviderSupportedAsync(provider))
            {
                return BadRequest();
            }

            // Instruct the middleware corresponding to the requested external identity
            // provider to redirect the user agent to its own authorization endpoint.
            var redirectUrl = Url.Action(nameof(ExternalRegisterCallback));
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        [HttpGet]
        public async Task<IActionResult> ExternalRegisterCallback()
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ModelState.AddModelError("", "please sign in");
                return RedirectToAction(nameof(Register));
            }

            #pragma warning disable CA5394 // Using a weak psuedo-random number generator is fine in this case.
            Random random = new Random();
            const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";

            var account = new Account
            {
                UserName = new string(Enumerable.Repeat(chars, 20).Select(s => s[random.Next(s.Length)]).ToArray()),
                Email = info.Principal.FindFirstValue(ClaimTypes.Email),
            };

            var createResult = await _signInManager.UserManager.CreateAsync(account);
            if (!createResult.Succeeded)
            {
                ModelState.AddModelError("", "could not create account");
                foreach (IdentityError error in createResult.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                    // TODO: implement view
                    return View();
                }
            }

            var bindResult = await _signInManager.UserManager.AddLoginAsync(account, info);
            if (!bindResult.Succeeded)
            {
                ModelState.AddModelError("", "could not bind account to external login info");
                foreach (IdentityError error in bindResult.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                    // TODO: implement view
                    return View();
                }
            }

            var signInResult = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            if (signInResult.Succeeded)
            {
                return RedirectToAction("Index", "App");
            }
            else if (signInResult.IsLockedOut)
            {
                ModelState.AddModelError("", "account locked; please contact the administrator");
                // TODO: implement an actual view so we can see the account error
                return View();
            }
            else
            {
                ModelState.AddModelError("", "could not sign you in.");
                return RedirectToAction(nameof(Login));
            }
        }

        [HttpGet]
        public async Task<IActionResult> Login()
        {
            TraceMethodEntry();

            if (this.User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "App");
            }
            var form = new LoginForm
            {
                AuthenticationSchemes = await HttpContext.GetExternalProvidersAsync(),
            };
            return View(form);
        }

        /// <summary>
        /// Log in with the provided username/password.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Login([FromForm] LoginForm form)
        {
            TraceMethodEntry(WithArgs(form));

            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(form.UserName, form.Password, form.RememberMe, false);
                await RecordLoginAttempt(EventOrigin.UI, form.UserName, result);

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

        /// <summary>
        /// Log in using the external login provider.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> ExternalLogin([FromForm] string provider, string returnUrl = "/")
        {
            TraceMethodEntry();

            // Note: the "provider" parameter corresponds to the external
            // authentication provider chosen by the user agent.
            if (string.IsNullOrWhiteSpace(provider))
            {
                return BadRequest();
            }

            if (!await HttpContext.IsProviderSupportedAsync(provider))
            {
                return BadRequest();
            }

            // Instruct the middleware corresponding to the requested external identity
            // provider to redirect the user agent to its own authorization endpoint.
            var redirectUrl = Url.Action(nameof(ExternalLoginCallback), new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        /// <summary>
        /// Login callback URL used after an external login provider has been challenged.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = "/")
        {
            TraceMethodEntry();

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToAction(nameof(Login));
            }

            var signInResult = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            if (signInResult.Succeeded)
            {
                return Redirect(returnUrl);
            }

            if (signInResult.IsLockedOut)
            {
                ModelState.AddModelError("", "account locked; please contact the administrator");
                // TODO: implement an actual view so we can see the account error
                return View();
            }

            ViewData["ReturnUrl"] = returnUrl;
            ViewData["Provider"] = info.LoginProvider;
            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            return RedirectToAction("Index", "App");
        }

        public async Task<IActionResult> Logout()
        {
            TraceMethodEntry();

            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "App");
        }

        [HttpPost]
        public async Task<IActionResult> CreateToken([FromBody] ApiLoginForm form)
        {
            TraceMethodEntry(WithArgs(form));

            if (ModelState.IsValid)
            {
                var user = await _signInManager.UserManager.FindByNameAsync(form.UserName);
                if (user != null)
                {
                    var result = await _signInManager.CheckPasswordSignInAsync(user, form.Password, lockoutOnFailure: false);
                    await RecordLoginAttempt(EventOrigin.API, form.UserName, result);

                    if (result.Succeeded)
                    {
                        _logger.LogTrace($"CreateToken {form.UserName}: sign in succeeded");

                        // create the token here
                        // Claims-based identity is a common way for applications to acquire the identity information they need about users inside their organization, in other organizations,
                        // and on the Internet. It also provides a consistent approach for applications running on-premises or in the cloud.
                        // Claims-based identity abstracts the individual elements of identity and access control into two parts:
                        //
                        // 1. a notion of claims, and
                        // 2. the concept of an issuer or an authority
                        //
                        // to create a claim you need a time and a value!
                        var claims = new[]
                        {
                            // Sub - name of the subject - which is user email here.
                            new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                            // jti - unique string that is representative of each token so using a guid
                            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                            // unique name - username of the user mapped to the identity inside the user object
                            // that is available on every controller and view
                            new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName)
                        };

                        // key is the secret used to encrypt the token. some parts of the token aren't encrypted but other parts are.
                        // credentials, who it is tied to and exploration etc are encrypted.
                        // information about the claims, about the individual etc aren't encrypted.
                        // use a natural string for a string and encode it to bytes.
                        // read from configuration json - keep changing/or fetch from another source.
                        // the trick here is that the key needs to be accessible for the application
                        // also needs to be replaceable by the people setting up your system.
                        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

                        // new credentials required. create it using the key you just created in combination with a
                        // security algorithm.
                        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                        var token = new JwtSecurityToken(_configuration["Jwt:Issuer"], // the creator of the token
                        _configuration["Jwt:Audience"], // who can use the token
                        claims,
                        expires: DateTime.UtcNow.AddMinutes(30),
                        signingCredentials: credentials);

                        var results = new
                        {
                            token = new JwtSecurityTokenHandler().WriteToken(token),
                            expiration = token.ValidTo
                        };

                        return Created(results);
                    }
                    else
                    {
                        _logger.LogWarning($"CreateToken {form.UserName}: sign in failed: {SigninFailureLogMessage(result)}");
                    }
                }
            }
            return BadRequest();
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

        private async Task RecordLoginAttempt(EventOrigin source, string userName, Microsoft.AspNetCore.Identity.SignInResult result)
        {
            if (result.Succeeded)
            {
                await _unitOfWork.EventLog.LoginSucceeded(source, userName);
            }
            else
            {
                await _unitOfWork.EventLog.LoginFailed(source, userName, SigninFailureLogMessage(result));
            }
            await _unitOfWork.SaveChanges();
        }
    }
}
