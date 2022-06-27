using Hippo.Application.Revisions.Queries;
using Hippo.Core.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hippo.Web.Api;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class StorageController : ApiControllerBase
{
    [HttpGet]
    public async Task<Page<string>> QueryStorages(
        [FromQuery] string searchText = "",
        [FromQuery] int pageIndex = 0,
        [FromQuery] int pageSize = 50,
        [FromQuery] bool IsSortedAscending = true)
    {
        return await Mediator.Send(new GetStoragesQuery()
        {
            SearchText = searchText,
            PageIndex = pageIndex,
            PageSize = pageSize,
            IsSortedAscending = IsSortedAscending
        });
    }
}
