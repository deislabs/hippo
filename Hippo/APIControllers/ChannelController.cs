using System;
using System.Threading.Tasks;
using Hippo.Controllers;
using Hippo.Messages;
using Hippo.Models;
using Hippo.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Hippo.APIControllers
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
        /// <summary>
        /// Initializes a new instance of the <see cref="ChannelController"/> class.
        /// </summary>
        /// <param name="unitOfWork">Iunitofwork instance</param>
        /// <param name="logger">ILogger Instance</param>
        public ChannelController(IUnitOfWork unitOfWork, ILogger<ChannelController> logger)
                : base(logger)
        {
            this._unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Creates a new Hippo Channel
        /// </summary>
        /// <remarks>
        /// Sample requests:
        ///
        ///     POST /api/channel
        ///     {
        ///        "appId": "4208d635-7618-4150-b8a8-bc3205e70e32",
        ///        "name": "My new Channel",
        ///        "fixedToRevision": true,
        ///        "revisionNumber": "1.2.3"
        ///     }
        ///     
        ///     POST /api/channel
        ///     {
        ///        "appId": "4208d635-7618-4150-b8a8-bc3205e70e32",
        ///        "name": "My new Channel",
        ///        "fixedToRevision": false,
        ///        "revisionRange": "~1.2.3"
        ///     }
        ///
        /// </remarks>
        /// <param name="request">The channel details.</param>
        /// <returns>Details of the newly created Channel.</returns>
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
                var channel = new Models.Channel
                {
                    Id = channelId,
                    Application = app,
                    Name = request.Name,
                    RevisionSelectionStrategy = request.FixedToRevision ? ChannelRevisionSelectionStrategy.UseSpecifiedRevision : ChannelRevisionSelectionStrategy.UseRangeRule,
                    RangeRule = request.FixedToRevision ? "" : request.RevisionRange,
                    SpecifiedRevision = request.FixedToRevision ? new Revision { RevisionNumber = request.RevisionNumber } : null
                };
                
                await _unitOfWork.Channels.AddNew(channel);
                await _unitOfWork.SaveChanges();


                //TODO: should the new channel be started?

                var response = new CreateChannelResponse()
                {
                   Id = channelId
                };
                return Created("", response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception Creating Application");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Gets a Hippo Channel biy Channel Id
        /// </summary>
        /// <remarks>
        /// Sample requests:
        ///
        ///     Get /api/channel/4208d635-7618-4150-b8a8-bc3205e70e32
        ///   
        ///
        /// </remarks>
        /// <param name="id">The channel id.</param>
        /// <returns>Details of the Channel.</returns>
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
                if (!Guid.TryParse(id, out Guid guid))
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
                    FixedToRevision = channel.RevisionSelectionStrategy == ChannelRevisionSelectionStrategy.UseSpecifiedRevision,
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
