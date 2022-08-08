using Hippo.Application.Certificates.Commands;
using Hippo.Application.Certificates.Queries;
using Hippo.Core.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hippo.Web.Api;

public class CertificatesController : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<Page<CertificateItem>>> Index(
        [FromQuery] string searchText = "",
        [FromQuery] int pageIndex = 0,
        [FromQuery] int pageSize = 50,
        [FromQuery] string sortBy = "Name",
        [FromQuery] bool IsSortedAscending = true)
    {
        return await Mediator.Send(new GetCertificatesQuery()
        {
            SearchText = searchText,
            PageIndex = pageIndex,
            PageSize = pageSize,
            SortBy = sortBy,
            IsSortedAscending = IsSortedAscending
        });
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> Create([FromBody] CreateCertificateCommand command)
    {
        return await Mediator.Send(command);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(Guid id, UpdateCertificateCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest();
        }

        await Mediator.Send(command);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        await Mediator.Send(new DeleteCertificateCommand { Id = id });

        return NoContent();
    }
}
