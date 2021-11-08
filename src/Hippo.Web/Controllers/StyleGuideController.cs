using Microsoft.AspNetCore.Mvc;

namespace Hippo.Web.Controllers
{
    public class StyleGuideController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
    }
}
