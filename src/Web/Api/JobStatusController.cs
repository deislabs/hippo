using Hippo.Application.Channels.Queries;
using Hippo.Core.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hippo.Web.Api;

public class JobStatusController : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<Page<ChannelJobStatusItem>>> Index(
        [FromQuery] int pageIndex = 0,
        [FromQuery] int pageSize = int.MaxValue)
    {
        return await Mediator.Send(new GetJobStatusesQuery()        
        {
            PageIndex = pageIndex,
            PageSize = pageSize,
        });
    }
}
