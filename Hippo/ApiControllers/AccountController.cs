using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Hippo.Controllers;
using Hippo.Messages;
using Hippo.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Hippo.ApiControllers
{
    /// <summary>
    /// AccountController providers an API to create Hippo Applications 
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : HippoController
    {
        private readonly SignInManager<Account> _signInManager;
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountController"/> class.
        /// </summary>
        /// <param name="signInManager">SignInManager instance</param>
        /// <param name="configuration">IConfiguration  instance</param>
        /// <param name="logger">ILogger Instance</param>
        public AccountController(SignInManager<Account> signInManager, IConfiguration configuration, ILogger<AccountController> logger)
            : base(logger)
        {
            this._signInManager = signInManager;
            this._configuration = configuration;
        }

        /// <summary>
        /// Gets a bearer token for use in API Requests.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/account
        ///     {
        ///        "userName": "user",
        ///        "password": "secretvalue"
        ///     }
        ///
        /// </remarks>
        /// <param name="request">The user credentials.</param>
        /// <returns> Token and expiration time.</returns>
        /// <response code="201">Returns the token details.</response>
        /// <response code="400">The request is invalid</response> 
        /// <response code="401">The request is unautorized</response>
        /// <response code="500">An error occured in the server when processing the request</response> 
        [HttpPost(Name = "GetToken")]
        [ProducesResponseType(typeof(Messages.GetTokenResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Consumes("application/json")]
        [Produces("application/json")]
        public async Task<ActionResult<GetTokenResponse>> GetToken(GetTokenRequest request)
        {
            try
            {
                TraceMethodEntry(WithArgs(request));

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var user = await _signInManager.UserManager.FindByNameAsync(request.UserName);
                if (user == null)
                {
                    return BadRequest();
                }
                var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    _logger.LogTrace($"CreateToken {request.UserName}: sign in succeeded");

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

                    var token = new JwtSecurityToken(
                        _configuration["Jwt:Issuer"], // the creator of the token
                        _configuration["Jwt:Audience"], // who can use the token
                        claims,
                        expires: DateTime.UtcNow.AddMinutes(30),
                        signingCredentials: credentials);

                    var results = new GetTokenResponse
                    {
                        Token = new JwtSecurityTokenHandler().WriteToken(token),
                        Expiration = token.ValidTo
                    };

                    return results;
                }
                else
                {
                    var reasons = SigninFailureLogMessage(result);
                    _logger.LogWarning($"CreateToken {request.UserName}: sign in failed: {reasons}");
                    return Unauthorized(reasons);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception Creating Application");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
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
