using Hippo.Application.Revisions.Commands;
using Hippo.Application.Revisions.Queries;
using Hippo.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace Hippo.Web.Api;

public class RevisionsController : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<Page<RevisionItem>>> Index(
        [FromQuery] int pageIndex = 0,
        [FromQuery] int pageSize = 50)
    {
        return await Mediator.Send(new GetRevisionsQuery()
        {
            PageIndex = pageIndex,
            PageSize = pageSize
        });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] RegisterRevisionCommand command)
    {
        var vm = await Mediator.Send(command);

        return vm.Revisions.Any() ? Created("", null) : NotFound();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await Mediator.Send(new DeleteRevisionCommand { Id = id });

        return NoContent();
    }
}
