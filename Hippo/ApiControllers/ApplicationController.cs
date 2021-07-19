using System;
using System.Threading.Tasks;
using Hippo.ControllerCore;
using Hippo.Controllers;
using Hippo.Messages;
using Hippo.Models;
using Hippo.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;


namespace Hippo.ApiControllers
{
    /// <summary>
    /// ApplicationController providers an API to create Hippo Applications 
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ApplicationController : HippoController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<Account> _userManager;
        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationController"/> class.
        /// </summary>
        /// <param name="unitOfWork">Iunitofwork instance</param>
        /// <param name="userManager">UserManager  instance</param>
        /// <param name="logger">ILogger Instance</param>
        public ApplicationController(IUnitOfWork unitOfWork, UserManager<Account> userManager, ILogger<ApplicationController> logger)
            : base(logger)
        {
            this._unitOfWork = unitOfWork;
            this._userManager = userManager;
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
        public async Task<ActionResult<Messages.CreateApplicationResponse>> New([FromBody] CreateApplicationRequest request)
        {
            try
            {
                TraceMethodEntry(WithArgs(request));

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var app = new Models.Application
                {
                    Id = System.Guid.NewGuid(),
                    Name = request.ApplicationName,
                    StorageId = request.StorageId,
                    Owner = await _userManager.FindByNameAsync(User.Identity.Name),
                };

                await _unitOfWork.Applications.AddNew(app);
                await _unitOfWork.SaveChanges();
                var response = new CreateApplicationResponse
                {
                    Id = app.Id,
                    ApplicationName = app.Name,
                    StorageId = app.StorageId
                };

                TraceMessage($"Successfully Created Application Id: {app.Id}");
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
