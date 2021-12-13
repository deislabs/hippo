using Hippo.Application.Channels.Commands;
using Hippo.Application.Channels.Queries;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hippo.Web.Api;

// [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class ChannelController : ApiControllerBase
{
    [HttpGet]
    [Route("")]
    public async Task<ActionResult<ChannelsVm>> Index()
    {
        return await Mediator.Send(new GetChannelsQuery());
    }

    [HttpGet]
    [Route("{id:int}")]
    public async Task<FileResult> Index(Guid id)
    {
        var vm = await Mediator.Send(new ExportChannelsQuery());

        return File(vm.Content, vm.ContentType, vm.FileName);
    }

    [HttpPost]
    [Route("")]
    public async Task<ActionResult<Guid>> Create([FromBody] CreateChannelCommand command)
    {
        return await Mediator.Send(command);
    }

    [HttpDelete]
    [Route("{id:int}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        await Mediator.Send(new DeleteChannelCommand { Id = id });

        return NoContent();
    }
}
