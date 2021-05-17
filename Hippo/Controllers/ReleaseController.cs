using System.Linq;
using System.Threading.Tasks;
using Hippo.Models;
using Hippo.ViewModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Hippo.Controllers
{
    [Route("api/[Controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ReleaseController : Controller
    {
        private readonly DataContext context;

        public ReleaseController(DataContext context)
        {
            this.context = context;
        }

        [HttpPost]
        public IActionResult New(ReleaseUploadForm form)
        {
            if (ModelState.IsValid)
            {
                var app = context.Applications.Where(application=>application.Id==form.AppId && application.Owner.UserName==User.Identity.Name).SingleOrDefault();
                if (app != null)
                {
                    app.Releases.Add(new Release
                    {
                        Revision = form.Revision,
                        UploadUrl = form.UploadUrl
                    });
                    context.SaveChanges();
                    return RedirectToAction("Index", "App");
                }
                else
                {
                    return NotFound();
                }

            }
            return NotFound();
        }
    }
}
