using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hippo.Core.Interfaces;
using Hippo.Core.Messages;
using Hippo.Core.Models;
using Hippo.Core.Tasks;
using Hippo.Infrastructure.Data;
using Hippo.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Hippo.Web.Controllers
{
    // TODO: This seems to be an API controller so needs swagger attributes and moving under APIControllers Directory

    [Route("api/[Controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class RevisionController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITaskQueue<ChannelReference> _channelsToReschedule;

        public RevisionController(IUnitOfWork unitOfWork, ITaskQueue<ChannelReference> channelsToReschedule, ILogger<RevisionController> logger)
            : base(logger)
        {
            this._unitOfWork = unitOfWork;
            this._channelsToReschedule = channelsToReschedule;
        }

        [HttpPost]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA5391: Method New handles a HttpPost request without performing antiforgery token validation.You also need to ensure that your HTML form sends an antiforgery token.", Justification = "This is used as an API.")]
        public async Task<IActionResult> New([FromBody] RegisterRevisionRequest request)
        {
            TraceMethodEntry(WithArgs(request));

            if (ModelState.IsValid)
            {
                var apps = FindApps();
                var changes = new List<ActiveRevisionChange>();

                foreach (var app in apps)
                {
                    // TODO: less worse handling of duplicate version
                    app.Revisions.Add(new Revision
                    {
                        RevisionNumber = request.RevisionNumber,
                    });

                    var appChannelChanges = app.ReevaluateActiveRevisions();
                    changes.AddRange(appChannelChanges);
                }

                foreach (var change in changes)
                {
                    await _unitOfWork.EventLog.ChannelRevisionChanged(EventOrigin.API, change.Channel, change.ChangedFrom, "registered revision");
                }

                await _unitOfWork.SaveChanges();

                var changedChannels = changes.Select(c => c.Channel).ToList().AsReadOnly();

                var queueRescheduleTasks = changedChannels.Select(c =>
                    _channelsToReschedule.Enqueue(new ChannelReference(c.Application.Id, c.Id), CancellationToken.None)
                );

                try
                {
                    await Task.WhenAll(queueRescheduleTasks);
                }
                catch (Exception e)
                {
                    _logger.LogError($"New: failed to queue channel rescheduling for one or more of {String.Join(",", changedChannels.Select(c => c.Name))}: {e}");
                    throw;
                }

                return apps.Any() ?
                    Created(null) :
                    NotFound();
            }

            return NotFound();

            IList<Application> FindApps()
            {
                if (request.AppId.HasValue)
                {
                    var app = _unitOfWork.Applications.GetApplicationById(request.AppId.Value);
                    LogIfNotFound(app, request.AppId);
                    return new[] { app };
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
                    return Array.Empty<Application>();
                }

            }
        }
    }
}
