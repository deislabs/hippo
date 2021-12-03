using Hippo.Application.Common.Security;
using Hippo.Application.EnvironmentVariables.Commands;
using Hippo.Application.EnvironmentVariables.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Hippo.Web.Api;

[Route("api/[controller]", Name = "Api[controller]")]
[ApiController]
[Authorize]
public class EnvironmentVariableController : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<EnvironmentVariablesVm>> Index()
    {
        return await Mediator.Send(new GetEnvironmentVariablesQuery());
    }

    [HttpGet]
    public async Task<FileResult> Index(Guid id)
    {
        var vm = await Mediator.Send(new ExportEnvironmentVariablesQuery());

        return File(vm.Content, vm.ContentType, vm.FileName);
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> Create(CreateEnvironmentVariableCommand command)
    {
        return await Mediator.Send(command);
    }

    [HttpDelete]
    public async Task<ActionResult> Delete(Guid id)
    {
        await Mediator.Send(new DeleteEnvironmentVariableCommand { Id = id });

        return NoContent();
    }
}
