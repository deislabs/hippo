using Hippo.Application.Channels.Commands;
using Hippo.Application.Channels.Queries;
using Hippo.Application.Common.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hippo.Web.Controllers;

[Authorize]
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
        try
        {
            var id = await Mediator.Send(command);
            return RedirectToAction(nameof(Details), new { id = id });
        }
        catch (ValidationException e)
        {
            ModelState.AddModelError("", e.Message);
            return View(command);
        }
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
    public async Task<IActionResult> Edit(Guid id)
    {
        try
        {
            ChannelDto dto = await Mediator.Send(new GetChannelQuery { Id = id });

            return View(new UpdateChannelCommand
            {
                Id = dto.Id,
                Name = dto.Name,
                Domain = dto.Domain,
                RevisionSelectionStrategy = dto.RevisionSelectionStrategy,
                RangeRule = dto.RangeRule,
                ActiveRevisionId = dto.ActiveRevision?.Id
            });
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

    [HttpPost]
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
