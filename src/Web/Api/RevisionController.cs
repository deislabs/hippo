using Hippo.Application.Revisions.Commands;
using Hippo.Application.Revisions.Queries;
using Hippo.Core.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hippo.Web.Api;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class RevisionController : ApiControllerBase
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

    [HttpGet("export")]
    public async Task<FileResult> Export()
    {
        var vm = await Mediator.Send(new ExportRevisionsQuery());

        return File(vm.Content, vm.ContentType, vm.FileName);
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
