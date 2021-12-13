using Hippo.Application.Domains.Commands;
using Hippo.Application.Domains.Queries;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hippo.Web.Api;

// [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class DomainController : ApiControllerBase
{
    [HttpGet]
    [Route("")]
    public async Task<ActionResult<DomainsVm>> Index()
    {
        return await Mediator.Send(new GetDomainsQuery());
    }

    [HttpGet]
    [Route("{id:int}")]
    public async Task<FileResult> Index(Guid id)
    {
        var vm = await Mediator.Send(new ExportDomainsQuery());

        return File(vm.Content, vm.ContentType, vm.FileName);
    }

    [HttpPost]
    [Route("")]
    public async Task<ActionResult<Guid>> Create([FromBody] CreateDomainCommand command)
    {
        return await Mediator.Send(command);
    }

    [HttpDelete]
    [Route("{id:int}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        await Mediator.Send(new DeleteDomainCommand { Id = id });

        return NoContent();
    }
}