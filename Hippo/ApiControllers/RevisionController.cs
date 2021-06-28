using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hippo.Controllers;
using Hippo.Messages;
using Hippo.Models;
using Hippo.Repositories;
using Hippo.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Hippo.ApiControllers
{
    /// <summary>
    ///  RevisionController provides an API to create Hippo Application Revisions.
    /// </summary>
    [Route("api/[Controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class RevisionController : HippoController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITaskQueue<ChannelReference> _channelsToReschedule;

        /// <summary>
        /// Initializes a new instance of the <see cref="RevisionController"/> class.
        /// </summary>
        /// <param name="unitOfWork">IUnitOfWork instance</param>
        /// <param name="channelsToReschedule">ITaskQueue instance</param>
        /// <param name="logger">ILogger instance,</param>
        public RevisionController(IUnitOfWork unitOfWork, ITaskQueue<ChannelReference> channelsToReschedule, ILogger<RevisionController> logger)
            : base(logger)
        {
            this._unitOfWork = unitOfWork;
            this._channelsToReschedule = channelsToReschedule;
        }

        /// <summary>
        /// Creates a new Hippo Application Revision
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/revision
        ///     {
        ///        "appId": "4208d635-7618-4150-b8a8-bc3205e70e32",
        ///        "revisionNumber": "1.2.3"
        ///     }
        ///
        ///     POST /api/revision
        ///     {
        ///        "storageId": "contoso/weather",
        ///        "revisionNumber": "1.2.3"
        ///     }
        ///
        /// </remarks>
        /// <param name="request">The application revision details.</param>
        [HttpPost(Name = "CreateRevision")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Consumes("application/json")]
        public async Task<IActionResult> New([FromBody] RegisterRevisionRequest request)
        {
            try
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
                        Created(null) :
                        NotFound();
                }

                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception Creating Application");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

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
