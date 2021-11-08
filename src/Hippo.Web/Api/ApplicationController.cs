using System;
using System.Threading.Tasks;
using Hippo.Core.Interfaces;
using Hippo.Core.Messages;
using Hippo.Core.Models;
using Hippo.Core.Tasks;
using Hippo.Infrastructure.Data;
using Hippo.Infrastructure.Services;
using Hippo.Web.Controllers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;


namespace Hippo.Web.Api
{
    /// <summary>
    /// ApplicationController providers an API to create Hippo Applications 
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ApplicationController : ApplicationBaseController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationController"/> class.
        /// </summary>
        /// <param name="unitOfWork">Iunitofwork instance</param>
        /// <param name="userManager">UserManager  instance</param>
        /// <param name="channelsToReschedule" />
        /// <param name="logger">ILogger Instance</param>
        public ApplicationController(IUnitOfWork unitOfWork, UserManager<Account> userManager, ITaskQueue<ChannelReference> channelsToReschedule, ILogger<ApplicationController> logger)
            : base(unitOfWork, userManager, channelsToReschedule, logger, EventOrigin.API)
        {
        }

        /// <summary>
        /// Creates a new Hippo Application
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/application
        ///     {
        ///        "applicationName": "My excellent new application",
        ///        "storageId": "contoso/weather"
        ///     }
        ///
        /// </remarks>
        /// <param name="request">The application details.</param>
        /// <returns>Details of the newly created Hippo Application.</returns>
        /// <response code="201">Returns the newly created Application details</response>
        /// <response code="400">The request is invalid</response> 
        /// <response code="500">An error occured in the server when processing the request</response> 
        [HttpPost(Name = "CreateHippoApplication")]
        [ProducesResponseType(typeof(CreateApplicationResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Consumes("application/json")]
        [Produces("application/json")]
        public async Task<ActionResult<CreateApplicationResponse>> New([FromBody] CreateApplicationRequest request)
        {
            try
            {
                TraceMethodEntry(WithArgs(request));

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await CreateApplication(request);

                if (result.Result != null)
                {
                    return result.Result;
                }

                var application = result.Value;
                TraceMessage($"Successfully Created Application Id: {application.Id}");

                var response = new CreateApplicationResponse
                {
                    Id = application.Id,
                    ApplicationName = application.Name,
                    StorageId = application.StorageId,
                };
                return Created(response);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception Creating Application Name:{request.ApplicationName} StorageId: {request.StorageId}");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
