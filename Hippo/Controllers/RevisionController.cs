using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hippo.Messages;
using Hippo.Models;
using Hippo.Repositories;
using Hippo.Schedulers;
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
        public async Task<IActionResult> New([FromBody] RegisterRevisionRequest request)
        {
            TraceMethodEntry(WithArgs(request));

            if (ModelState.IsValid)
            {
                var apps = FindApps();

                foreach (var app in apps)
                {
                    // TODO: less worse handling of duplicate version
                    app.Revisions.Add(new Revision
                    {
                        RevisionNumber = request.RevisionNumber,
                    });
                    // TODO: same comment as on app controller, and this really feels out of place
                    // should we raise events on active revision changes and let schedulers subscribe?
                    // (or even do reevaluation and/or channel update as a background process)
                    foreach (var channel in app.ReevaluateActiveRevisions())
                    {
                        _scheduler.Stop(channel);
                        _scheduler.Start(channel);
                    }
                }

                await _unitOfWork.SaveChanges();

                return apps.Any() ?
                    Created("", null) :
                    NotFound();
            }

            return NotFound();

            IList<Application> FindApps()
            {
                if (request.AppId.HasValue)
                {
                    var app = _unitOfWork.Applications.GetApplicationById(request.AppId.Value);
                    LogIfNotFound(app, request.AppId);
                    return new [] { app };
                }
                else if (request.AppStorageId != null)
                {
                    var apps = _unitOfWork.Applications.ListApplicationsByStorageId(request.AppStorageId).ToList();
                    if (apps.Count == 0)
                    {
                        _logger.LogDebug($"Register Revision: no apps found for {request.AppStorageId}");
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
