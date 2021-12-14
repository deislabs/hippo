using Microsoft.AspNetCore.Mvc;

namespace Hippo.Web.Controllers;

public class StyleguideController : WebUIControllerBase
{
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }
}
