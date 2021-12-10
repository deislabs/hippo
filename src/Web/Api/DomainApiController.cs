using Hippo.Application.Common.Security;
using Hippo.Application.Domains.Commands;
using Hippo.Application.Domains.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Hippo.Web.Api;

[Route("api/domain/[action]")]
[ApiController]
[Authorize]
public class DomainApiController : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<DomainsVm>> Index()
    {
        return await Mediator.Send(new GetDomainsQuery());
    }

    [HttpGet]
    public async Task<FileResult> Index(Guid id)
    {
        var vm = await Mediator.Send(new ExportDomainsQuery());

        return File(vm.Content, vm.ContentType, vm.FileName);
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> Create(CreateDomainCommand command)
    {
        return await Mediator.Send(command);
    }

    [HttpDelete]
    public async Task<ActionResult> Delete(Guid id)
    {
        await Mediator.Send(new DeleteDomainCommand { Id = id });

        return NoContent();
    }
}
