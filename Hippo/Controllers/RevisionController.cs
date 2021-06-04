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
    public class RevisionController : HippoController
    {
        private readonly IUnitOfWork _unitOfWork;

        public RevisionController(IUnitOfWork unitOfWork, ILogger<RevisionController> logger)
            : base(logger)
        {
            this._unitOfWork = unitOfWork;
        }

        [HttpPost]
        public async Task<IActionResult> New(RevisionRegistrationForm form)
        {
            TraceMethodEntry(WithArgs(form));

            if (ModelState.IsValid)
            {
                var app = _unitOfWork.Applications.GetApplicationById(form.AppId);
                LogIfNotFound(app, form.AppId);

                if (app != null)
                {
                    app.Revisions.Add(new Revision
                    {
                        RevisionNumber = form.RevisionNumber,
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
