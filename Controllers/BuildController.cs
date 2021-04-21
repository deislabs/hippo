using Hippo.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Hippo.Controllers
{
    [Route("api/[Controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class BuildController : Controller
    {
        private readonly UserManager<Account> userManager;

        public BuildController(UserManager<Account> userManager)
        {
            this.userManager = userManager;
        }
    }
}
