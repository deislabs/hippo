using Hippo.Application.Apps.Commands;
using Hippo.Application.Apps.Queries;
using Hippo.Application.Common.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hippo.Web.Controllers;

[Authorize]
public class AppController : WebUIControllerBase
{
    [HttpGet]
    public async Task<ActionResult<AppsVm>> Index()
    {
        AppsVm vm = await Mediator.Send(new GetAppsQuery());

        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid id)
    {
        try
        {
            AppDto dto = await Mediator.Send(new GetAppQuery { Id = id });

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
            AppDto dto = await Mediator.Send(new GetAppQuery { Id = id });

            return View(new UpdateAppCommand
            {
                Id = dto.Id,
                Name = dto.Name,
                StorageId = dto.StorageId
            });
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost]
    public async Task<IActionResult> Edit(UpdateAppCommand command)
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

    [HttpGet]
    public IActionResult New()
    {
        return View(new CreateAppCommand());
    }

    [HttpPost]
    public async Task<ActionResult<int>> New(CreateAppCommand command)
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
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            AppDto dto = await Mediator.Send(new GetAppQuery { Id = id });

            return View(dto);
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost]
    public async Task<IActionResult> Delete(DeleteAppCommand command)
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
