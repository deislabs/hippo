using Hippo.Application.Channels.Commands;
using Hippo.Application.Channels.Queries;
using Hippo.Application.Common.Security;
using Microsoft.AspNetCore.Mvc;

namespace Hippo.Web.Api;

[Route("api/channel/[action]")]
[ApiController]
[Authorize]
public class ChannelApiController : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ChannelsVm>> Index()
    {
        return await Mediator.Send(new GetChannelsQuery());
    }

    [HttpGet]
    public async Task<FileResult> Index(Guid id)
    {
        var vm = await Mediator.Send(new ExportChannelsQuery());

        return File(vm.Content, vm.ContentType, vm.FileName);
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> Create(CreateChannelCommand command)
    {
        return await Mediator.Send(command);
    }

    [HttpDelete]
    public async Task<ActionResult> Delete(Guid id)
    {
        await Mediator.Send(new DeleteChannelCommand { Id = id });

        return NoContent();
    }
}
