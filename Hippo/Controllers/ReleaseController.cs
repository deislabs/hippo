using System.Linq;
using System.Threading.Tasks;
using Hippo.Models;
using Hippo.Repositories;
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
        private readonly IUnitOfWork unitOfWork;

        public ReleaseController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        [HttpPost]
        public async Task<IActionResult> New(ReleaseUploadForm form)
        {
            if (ModelState.IsValid)
            {
                var app = unitOfWork.Applications.GetApplicationById(form.AppId);
                if (app != null)
                {
                    app.Releases.Add(new Release
                    {
                        Revision = form.Revision,
                        UploadUrl = form.UploadUrl
                    });
                    await unitOfWork.SaveChanges();
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
