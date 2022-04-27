using Hippo.Application.Revisions.Commands;
using Hippo.Application.Revisions.Queries;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hippo.Web.Api;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class StorageController : ApiControllerBase
{
    [HttpGet("export")]
    public async Task<ExportStoragesQueryVm> Export([FromQuery] string queryString,
        [FromQuery] ulong? offset,
        [FromQuery] int? limit)
    {
        return await Mediator.Send(new ExportStoragesQuery(queryString, offset, limit));
    }
}
