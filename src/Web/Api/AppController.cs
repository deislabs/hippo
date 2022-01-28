using Hippo.Application.Apps.Commands;
using Hippo.Application.Apps.Queries;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hippo.Web.Api;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class AppController : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<AppsVm>> Index()
    {
        return await Mediator.Send(new GetAppsQuery());
    }

    [HttpGet("export")]
    public async Task<FileResult> Export()
    {
        var vm = await Mediator.Send(new ExportAppsQuery());

        return File(vm.Content, vm.ContentType, vm.FileName);
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> Create([FromBody] CreateAppCommand command)
    {
        return await Mediator.Send(command);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        await Mediator.Send(new DeleteAppCommand { Id = id });

        return NoContent();
    }
}
