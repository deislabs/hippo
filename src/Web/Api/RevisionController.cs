using Hippo.Application.Revisions.Commands;
using Hippo.Application.Revisions.Queries;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hippo.Web.Api;

// [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class RevisionController : ApiControllerBase
{
    [HttpGet]
    [Route("")]
    public async Task<ActionResult<RevisionsVm>> Index()
    {
        return await Mediator.Send(new GetRevisionsQuery());
    }

    [HttpGet]
    [Route("{id:int}")]
    public async Task<FileResult> Index(Guid id)
    {
        var vm = await Mediator.Send(new ExportRevisionsQuery());

        return File(vm.Content, vm.ContentType, vm.FileName);
    }

    [HttpPost]
    [Route("")]
    public async Task<IActionResult> Create([FromBody] RegisterRevisionCommand command)
    {
        var vm = await Mediator.Send(command);

        return vm.Revisions.Any() ? Created("", null) : NotFound();
    }

    [HttpDelete]
    [Route("{id:int}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await Mediator.Send(new DeleteRevisionCommand { Id = id });

        return NoContent();
    }
}
