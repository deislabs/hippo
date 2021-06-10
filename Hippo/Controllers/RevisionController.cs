using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hippo.Models;
using Hippo.Repositories;
using Hippo.Schedulers;
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
        private readonly IJobScheduler _scheduler;

        public RevisionController(IUnitOfWork unitOfWork, IJobScheduler scheduler, ILogger<RevisionController> logger)
            : base(logger)
        {
            this._unitOfWork = unitOfWork;
            this._scheduler = scheduler;
        }

        [HttpPost]
        public async Task<IActionResult> New(RevisionRegistrationForm form)
        {
            TraceMethodEntry(WithArgs(form));

            if (ModelState.IsValid)
            {
                var apps = FindApps();

                foreach (var app in apps)
                {
                    app.Revisions.Add(new Revision
                    {
                        RevisionNumber = form.RevisionNumber,
                    });
                    // TODO: same comment as on app controller, and this really feels out of place
                    // should we raise events on active revision changes and let schedulers subscribe?
                    foreach (var channel in app.ReevaluateActiveRevisions())
                    {
                        _scheduler.Stop(channel);
                        _scheduler.Start(channel);
                    }
                }

                await _unitOfWork.SaveChanges();

                return apps.Any() ?
                    Ok() : // TODO: would like it to be Created() but could create under multiple apps
                    NotFound();
            }

            return NotFound();

            IList<Application> FindApps()
            {
                if (form.AppId.HasValue)
                {
                    var app = _unitOfWork.Applications.GetApplicationById(form.AppId.Value);
                    LogIfNotFound(app, form.AppId);
                    return new [] { app };
                }
                else if (form.AppStorageId != null)
                {
                    var apps = _unitOfWork.Applications.ListApplicationsByStorageId(form.AppStorageId).ToList();
                    if (apps.Count == 0)
                    {
                        _logger.LogDebug($"Register Revision: no apps found for {form.AppStorageId}");
                    }
                    return apps;
                }
                else
                {
                    _logger.LogWarning($"Register Revision: neither app id nor storage id provided");
                    return new Application[0];
                }

            }
        }
    }
}
