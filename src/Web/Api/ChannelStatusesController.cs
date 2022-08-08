using Hippo.Application.ChannelStatuses.Queries;
using Hippo.Core.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hippo.Web.Api;

public class ChannelStatusesController : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<Page<ChannelJobStatusItem>>> Index(
        [FromQuery] int pageIndex = 0,
        [FromQuery] int pageSize = int.MaxValue,
        [FromQuery] Guid? channelId = null)
    {
        return await Mediator.Send(new GetChannelStatusesQuery()
        {
            PageIndex = pageIndex,
            PageSize = pageSize,
            ChannelId = channelId
        });
    }
}
