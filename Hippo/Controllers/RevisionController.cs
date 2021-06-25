using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hippo.Messages;
using Hippo.Models;
using Hippo.Repositories;
using Hippo.Schedulers;
using Hippo.Tasks;
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
        private readonly ITaskQueue<ChannelReference> _channelsToReschedule;

        public RevisionController(IUnitOfWork unitOfWork, ITaskQueue<ChannelReference> channelsToReschedule, ILogger<RevisionController> logger)
            : base(logger)
        {
            this._unitOfWork = unitOfWork;
            this._channelsToReschedule = channelsToReschedule;
        }

        [HttpPost]
        public async Task<IActionResult> New([FromBody] RegisterRevisionRequest request)
        {
            TraceMethodEntry(WithArgs(request));

            if (ModelState.IsValid)
            {
                var apps = FindApps();
                var changedChannels = new List<Channel>();

                foreach (var app in apps)
                {
                    // TODO: less worse handling of duplicate version
                    app.Revisions.Add(new Revision
                    {
                        RevisionNumber = request.RevisionNumber,
                    });

                    var changedAppChannels = app.ReevaluateActiveRevisions();
                    changedChannels.AddRange(changedAppChannels);
                }

                await _unitOfWork.SaveChanges();

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
                    return new Application[0];
                }

            }
        }
    }
}
