using Microsoft.AspNetCore.Mvc;

namespace Hippo.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
