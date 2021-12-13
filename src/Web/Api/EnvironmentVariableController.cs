using Hippo.Application.EnvironmentVariables.Commands;
using Hippo.Application.EnvironmentVariables.Queries;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hippo.Web.Api;

// [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class EnvironmentVariableController : ApiControllerBase
{
    [HttpGet]
    [Route("")]
    public async Task<ActionResult<EnvironmentVariablesVm>> Index()
    {
        return await Mediator.Send(new GetEnvironmentVariablesQuery());
    }

    [HttpGet]
    [Route("{id:int}")]
    public async Task<FileResult> Index(Guid id)
    {
        var vm = await Mediator.Send(new ExportEnvironmentVariablesQuery());

        return File(vm.Content, vm.ContentType, vm.FileName);
    }

    [HttpPost]
    [Route("")]
    public async Task<ActionResult<Guid>> Create([FromBody] CreateEnvironmentVariableCommand command)
    {
        return await Mediator.Send(command);
    }

    [HttpDelete]
    [Route("{id:int}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        await Mediator.Send(new DeleteEnvironmentVariableCommand { Id = id });

        return NoContent();
    }
}
