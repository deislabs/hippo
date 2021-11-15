using Microsoft.AspNetCore.Mvc;

namespace Hippo.Controllers;

public class StyleguideController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }
}
