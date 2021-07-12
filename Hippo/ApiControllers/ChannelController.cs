using System;
using System.Threading;
using System.Threading.Tasks;
using Hippo.Controllers;
using Hippo.Messages;
using Hippo.Models;
using Hippo.Repositories;
using Hippo.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Hippo.ApiControllers
{
    /// <summary>
    /// ChannelController providers an API to create Hippo Channels 
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ChannelController : HippoController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITaskQueue<ChannelReference> _taskQueue;
        /// <summary>
        /// Initializes a new instance of the <see cref="ChannelController"/> class.
        /// </summary>
        /// <param name="unitOfWork">Iunitofwork instance</param>
        /// <param name="taskQueue"> ITaskQueue instance</param>
        /// <param name="logger">ILogger Instance</param>
        public ChannelController(IUnitOfWork unitOfWork, ITaskQueue<ChannelReference> taskQueue, ILogger<ChannelController> logger)
                : base(logger)
        {
            _unitOfWork = unitOfWork;
            _taskQueue = taskQueue;
        }

        /// <summary>
        /// Creates a new Hippo ChannelMessage
        /// </summary>
        /// <remarks>
        /// Sample requests:
        ///
        ///     POST /api/channel
        ///     {
        ///        "appId": "4208d635-7618-4150-b8a8-bc3205e70e32",
        ///        "name": "My new ChannelMessage",
        ///        "revisionSelectionStrategy": "UseRangeRule",
        ///        "revisionNumber": "1.2.3"
        ///     }
        ///     
        ///     POST /api/channel
        ///     {
        ///        "appId": "4208d635-7618-4150-b8a8-bc3205e70e32",
        ///        "revisionSelectionStrategy": "UseSpecifiedRevision",
        ///        "fixedToRevision": false,
        ///        "revisionRange": "~1.2.3"
        ///     }
        ///
        /// </remarks>
        /// <param name="request">The channel details.</param>
        /// <returns>Details of the newly created ChannelMessage.</returns>
        /// <response code="201">Returns the newly created ApplicationMessage details</response>
        /// <response code="400">The request is invalid</response> 
        /// <response code="404">App was not found with the appId </response> 
        /// <response code="500">An error occured in the server when processing the request</response> 
        [HttpPost(Name = "CreateHippoChannel")]
        [ProducesResponseType(typeof(Messages.CreateChannelResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Consumes("application/json")]
        [Produces("application/json")]
        public async Task<ActionResult<Messages.CreateChannelResponse>> New([FromBody] CreateChannelRequest request)
        {
            try
            {
                TraceMethodEntry(WithArgs(request));
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var app = _unitOfWork.Applications.GetApplicationById(request.AppId);
                LogIfNotFound(app, request.AppId);
                if (app == null)
                {
                    return NotFound();
                }

                var channelId = System.Guid.NewGuid();
                var domain = new Models.Domain
                {
                    Name = request.Domain
                };
                var channel = new Models.Channel
                {
                    Id = channelId,
                    Application = app,
                    Name = request.Name,
                    Domain = domain,
                    RevisionSelectionStrategy = request.RevisionSelectionStrategy,
                    RangeRule = request.RevisionSelectionStrategy == ChannelRevisionSelectionStrategy.UseRangeRule ? request.RevisionRange : "",
                    SpecifiedRevision = request.RevisionSelectionStrategy == ChannelRevisionSelectionStrategy.UseSpecifiedRevision ? new Revision { RevisionNumber = request.RevisionNumber } : null
                };
                channel.ReevaluateActiveRevision();

                await _unitOfWork.Channels.AddNew(channel);
                await _unitOfWork.EventLog.ChannelCreated(EventOrigin.API, channel);
                await _unitOfWork.EventLog.ChannelRevisionChanged(EventOrigin.API, channel, "(none)", "channel created");
                await _unitOfWork.SaveChanges();

                await _taskQueue.Enqueue(new ChannelReference(channel.Application.Id, channel.Id), CancellationToken.None);

                var response = new CreateChannelResponse()
                {
                    Id = channelId,
                    AppId = app.Id,
                    Name = request.Name,
                    Domain = domain.Name,
                    RevisionNumber = request.RevisionNumber,
                    RevisionRange = request.RevisionRange,
                    RevisionSelectionStrategy = request.RevisionSelectionStrategy
                };
                return Created(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception Creating Channel");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Gets a Hippo ChannelMessage by ChannelMessage Id
        /// </summary>
        /// <remarks>
        /// Sample requests:
        ///
        ///     Get /api/channel/4208d635-7618-4150-b8a8-bc3205e70e32
        ///   
        ///
        /// </remarks>
        /// <param name="id">The channel id.</param>
        /// <returns>Details of the ChannelMessage.</returns>
        /// <response code="200">Returns the newly created ApplicationMessage details</response>
        /// <response code="400">The request is invalid</response> 
        /// <response code="404">App was not found with the appId </response> 
        /// <response code="500">An error occured in the server when processing the request</response> 
        [HttpGet(Name = "GetHippoChannelById")]
        [ProducesResponseType(typeof(Messages.GetChannelResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        public ActionResult<Messages.GetChannelResponse> GetById(string id)
        {
            try
            {
                TraceMethodEntry(WithArgs(id));
                if (!Guid.TryParse(id, out var guid))
                {
                    return BadRequest($"{id} is not a valid Identifier");
                }

                var channel = _unitOfWork.Channels.GetChannelById(guid);
                LogIfNotFound(channel, id);
                if (channel == null)
                {
                    return NotFound();
                }
                var response = new Messages.GetChannelResponse
                {
                    AppId = channel.Application.Id,
                    RevisionSelectionStrategy = channel.RevisionSelectionStrategy,
                    Name = channel.Name,
                    RevisionNumber = channel.SpecifiedRevision.RevisionNumber,
                    RevisionRange = channel.RangeRule
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception Getting Channel By Id {id}");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}

