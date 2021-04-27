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
    public class BuildController : Controller
    {
        private readonly IAppRepository repository;
        public BuildController(IAppRepository repository)
        {
            this.repository = repository;
        }

        [HttpPost]
        public IActionResult New(BuildUploadForm form)
        {
            if (ModelState.IsValid)
            {
                var app = repository.SelectByUserAndId(User.Identity.Name, form.AppId);
                if (app != null)
                {
                    var last_release = app.Releases.OrderBy(r => r.Revision).LastOrDefault();
                    app.Releases.Add(
                        new Release
                        {
                            Build = new Build
                            {
                                UploadUrl = form.UploadUrl
                            },
                            Config = last_release.Config,
                            Revision = last_release.Revision + 1,
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
