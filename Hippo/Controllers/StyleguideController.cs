using Microsoft.AspNetCore.Mvc;

namespace Hippo.Controllers
{
    public class StyleguideController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
