using Hippo.Application.Common.Security;
using Hippo.Application.Revisions.Commands;
using Hippo.Application.Revisions.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Hippo.Web.Api;

[Route("api/[controller]", Name = "Api[controller]")]
[ApiController]
[Authorize]
public class RevisionController : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<RevisionsVm>> Index()
    {
        return await Mediator.Send(new GetRevisionsQuery());
    }

    [HttpGet]
    public async Task<FileResult> Index(Guid id)
    {
        var vm = await Mediator.Send(new ExportRevisionsQuery());

        return File(vm.Content, vm.ContentType, vm.FileName);
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> Create(CreateRevisionCommand command)
    {
        return await Mediator.Send(command);
    }

    [HttpDelete]
    public async Task<ActionResult> Delete(Guid id)
    {
        await Mediator.Send(new DeleteRevisionCommand { Id = id });

        return NoContent();
    }
}
