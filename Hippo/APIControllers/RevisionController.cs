using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hippo.Controllers;
using Hippo.Messages;
using Hippo.Models;
using Hippo.Repositories;
using Hippo.Schedulers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Hippo.APIControllers
{
    /// <summary>
    /// RevisionController provides an API to create Hippo Application Revisions. 
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class RevisionController : HippoController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJobScheduler _scheduler;
        /// <summary>
        /// Initializes a new instance of the <see cref="RevisionController"/> class.
        /// </summary>
        /// <param name="unitOfWork">Iunitofwork instance</param>
        /// <param name="scheduler">IJobScheduler  instance</param>
        /// <param name="logger">ILogger Instance</param>
        public RevisionController(IUnitOfWork unitOfWork, IJobScheduler scheduler, ILogger<RevisionController> logger)
            : base(logger)
        {
            this._unitOfWork = unitOfWork;
            this._scheduler = scheduler;
        }

        /// <summary>
        /// Registers a new Hippo Application Revision
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/revision/New
        ///     {
        ///         "appId": "4208d635-7618-4150-b8a8-bc3205e70e32",
        ///         "appStorageId": "contoso/weather",
        ///         "revisionNumber": "v1.1.1"
        ///     }
        ///
        /// </remarks>
        /// <param name="request">The revision details.</param>
        /// <returns>Details of the newly created Revision.</returns>
        /// <response code="201">The revision was created.</response>
        /// <response code="400">The request is invalid</response> 
        /// <response code="404">No apps were found with the appId or appStorageId</response> 
        /// <response code="500">An error occured in the server when processing the request</response> 
        [HttpPost(Name = "CreateRevision")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Consumes("application/json")]
        [Produces("application/json")]
        public async Task<IActionResult> New([FromBody] RegisterRevisionRequest request)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception Creating Revision");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
