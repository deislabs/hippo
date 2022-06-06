using Hippo.Application.Channels.Queries;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hippo.Web.Api;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class JobStatusController : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<JobStatus>>> Index()
    {
        return await Mediator.Send(new GetJobStatusesQuery());
    }
}
