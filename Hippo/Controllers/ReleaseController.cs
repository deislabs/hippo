using System.Linq;
using System.Threading.Tasks;
using Hippo.Models;
using Hippo.Repositories;
using Hippo.ViewModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Hippo.Controllers
{
    [Route("api/[Controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ReleaseController : HippoController
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReleaseController(IUnitOfWork unitOfWork, ILogger<ReleaseController> logger)
            : base(logger)
        {
            this._unitOfWork = unitOfWork;
        }

        [HttpPost]
        public async Task<IActionResult> New(ReleaseUploadForm form)
        {
            TraceMethodEntry(WithArgs(form));

            if (ModelState.IsValid)
            {
                var app = _unitOfWork.Applications.GetApplicationById(form.AppId);
                LogIfNotFound(app, form.AppId);

                if (app != null)
                {
                    app.Releases.Add(new Release
                    {
                        Revision = form.Revision,
                        UploadUrl = form.UploadUrl
                    });
                    await _unitOfWork.SaveChanges();
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
