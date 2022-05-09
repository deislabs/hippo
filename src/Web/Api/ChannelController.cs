using Hippo.Application.Channels.Commands;
using Hippo.Application.Channels.Queries;
using Hippo.Application.Revisions.Queries;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hippo.Web.Api;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class ChannelController : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ChannelsVm>> Index()
    {
        return await Mediator.Send(new GetChannelsQuery());
    }

    [HttpGet("{channelId}/active-revision")]
    public async Task<ActionResult<RevisionDetailsVm?>> GetActiveRevisionDetails(Guid channelId)
    {
        return await Mediator.Send(new GetActiveRevisionDetailsQuery { ChannelId = channelId });
    }

    [HttpGet("export")]
    public async Task<FileResult> Export()
    {
        var vm = await Mediator.Send(new ExportChannelsQuery());

        return File(vm.Content, vm.ContentType, vm.FileName);
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> Create([FromBody] CreateChannelCommand command)
    {
        return await Mediator.Send(command);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(Guid id, UpdateChannelCommand command)
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
        await Mediator.Send(new DeleteChannelCommand { Id = id });

        return NoContent();
    }
}
