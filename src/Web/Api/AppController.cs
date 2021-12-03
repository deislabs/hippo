using Hippo.Application.Apps.Commands;
using Hippo.Application.Apps.Queries;
using Hippo.Application.Common.Security;
using Microsoft.AspNetCore.Mvc;

namespace Hippo.Web.Api;

[Route("api/[controller]", Name = "Api[controller]")]
[ApiController]
[Authorize]
public class AppController : ApiControllerBase
{
    public async Task<ActionResult<AppsVm>> Index()
    {
        return await Mediator.Send(new GetAppsQuery());
    }

    public async Task<FileResult> Export()
    {
        var vm = await Mediator.Send(new ExportAppsQuery());

        return File(vm.Content, vm.ContentType, vm.FileName);
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> Create(CreateAppCommand command)
    {
        return await Mediator.Send(command);
    }

    [HttpDelete]
    public async Task<ActionResult> Delete(DeleteAppCommand command)
    {
        await Mediator.Send(command);

        return NoContent();
    }
}
