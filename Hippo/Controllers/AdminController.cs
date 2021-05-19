using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hippo.Controllers
{
    [Authorize(Roles="Administrator")]
    public class AdminController : Controller
    {
        public IActionResult Terminal()
        {
            return View();
        }
    }
}
