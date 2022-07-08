using Hippo.Application.Channels.Commands;
using Hippo.Application.Channels.Queries;
using Hippo.Application.EnvironmentVariables.Commands;
using Hippo.Core.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hippo.Web.Api;

public class ChannelController : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<Page<ChannelItem>>> Index(
        [FromQuery] string searchText = "",
        [FromQuery] int pageIndex = 0,
        [FromQuery] int pageSize = 50,
        [FromQuery] string sortBy = "Name",
        [FromQuery] bool IsSortedAscending = true)
    {
        return await Mediator.Send(new GetChannelsQuery()
        {
            SearchText = searchText,
            PageIndex = pageIndex,
            PageSize = pageSize,
            SortBy = sortBy,
            IsSortedAscending = IsSortedAscending
        });
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ChannelItem>> GetChannel(Guid id)
    {
        return await Mediator.Send(new GetChannelQuery { Id = id });
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

    [HttpPatch("{id}")]
    public async Task<ActionResult> Patch([FromRoute] Guid id,
        [FromBody] PatchChannelCommand command)
    {
        command.ChannelId = id;
        await Mediator.Send(command);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        await Mediator.Send(new DeleteChannelCommand { Id = id });

        return NoContent();
    }

    [HttpGet("logs/{id}")]
    public async Task<GetChannelLogsVm> Logs([FromRoute] Guid id)
    {
        return await Mediator.Send(new GetChannelLogsQuery(id));
    }
}
