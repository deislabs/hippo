using Hippo.Application.EnvironmentVariables.Commands;
using Hippo.Application.EnvironmentVariables.Queries;
using Hippo.Application.Common.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace Hippo.Web.Controllers;

[Authorize]
public class EnvironmentVariableController : WebUIControllerBase
{
    [HttpGet]
    public IActionResult New([FromQuery] Guid channelId)
    {
        return View(new CreateEnvironmentVariableCommand { ChannelId = channelId });
    }

    [HttpPost]
    public async Task<IActionResult> New(CreateEnvironmentVariableCommand command)
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
            EnvironmentVariableDto dto = await Mediator.Send(new GetEnvironmentVariableQuery { Id = id });

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
            EnvironmentVariableDto dto = await Mediator.Send(new GetEnvironmentVariableQuery { Id = id });

            return View(new UpdateEnvironmentVariableCommand
            {
                Id = dto.Id,
                Key = dto.Key,
                Value = dto.Value
            });
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost]
    public async Task<IActionResult> Edit(UpdateEnvironmentVariableCommand command)
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
            EnvironmentVariableDto dto = await Mediator.Send(new GetEnvironmentVariableQuery { Id = id });

            return View(dto);
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(DeleteEnvironmentVariableCommand command)
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
