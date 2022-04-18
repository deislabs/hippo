using Hippo.Application.EnvironmentVariables.Commands;
using Hippo.Application.EnvironmentVariables.Queries;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hippo.Web.Api;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class EnvironmentVariableController : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<EnvironmentVariablesVm>> Index()
    {
        return await Mediator.Send(new GetEnvironmentVariablesQuery());
    }

    [HttpGet("export")]
    public async Task<FileResult> Export()
    {
        var vm = await Mediator.Send(new ExportEnvironmentVariablesQuery());

        return File(vm.Content, vm.ContentType, vm.FileName);
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> Create([FromBody] CreateEnvironmentVariableCommand command)
    {
        return await Mediator.Send(command);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(Guid id, UpdateEnvironmentVariableCommand command)
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
        await Mediator.Send(new DeleteEnvironmentVariableCommand { Id = id });

        return NoContent();
    }
}
