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
        private readonly IAppRepository repository;
        public ReleaseController(IAppRepository repository)
        {
            this.repository = repository;
        }

        [HttpPost]
        public IActionResult New(ReleaseUploadForm form)
        {
            if (ModelState.IsValid)
            {
                var app = repository.SelectByUserAndId(User.Identity.Name, form.AppId);
                if (app != null)
                {
                    app.Releases.Add(
                        new Release
                        {
                            Revision = form.Revision,
                            UploadUrl = form.UploadUrl
                        }
                    );
                    repository.Update(app);
                    repository.Save();
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
