using Hippo.Application.Channels.Commands;
using Hippo.Application.Channels.Queries;
using Hippo.Application.Common.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Hippo.Web.Controllers;

public class ChannelController : WebUIControllerBase
{
    [HttpGet]
    public IActionResult New([FromQuery] Guid appId)
    {
        return View(new CreateChannelCommand { AppId = appId });
    }

    [HttpPost]
    public async Task<IActionResult> New(CreateChannelCommand command)
    {
        // TODO: handle validation errors
        var id = await Mediator.Send(command);
        return RedirectToAction(nameof(Details), new { id = id });
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid id)
    {
        try
        {
            ChannelDto dto = await Mediator.Send(new GetChannelQuery { Id = id });

            return View(dto);
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }

    [HttpGet]
    public IActionResult Edit(Guid id)
    {
        try
        {
            return View(new GetChannelQuery { Id = id });
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost]
    public async Task<IActionResult> Edit(UpdateChannelCommand command)
    {
        try
        {
            await Mediator.Send(command);

            return RedirectToAction(nameof(Details), new { id = command.Id });
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }

    [HttpGet]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            ChannelDto dto = await Mediator.Send(new GetChannelQuery { Id = id });

            return View(dto);
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(DeleteChannelCommand command)
    {
        try
        {
            await Mediator.Send(command);

            return RedirectToAction(nameof(Index));
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }
}
