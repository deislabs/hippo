using Microsoft.AspNetCore.Mvc;

namespace Hippo.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
    }
}
