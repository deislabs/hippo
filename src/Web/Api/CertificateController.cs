using Hippo.Application.Certificates.Commands;
using Hippo.Application.Certificates.Queries;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hippo.Web.Api;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class CertificateController : ApiControllerBase
{
    [HttpGet]
    [Route("")]
    public async Task<ActionResult<CertificatesVm>> Index()
    {
        return await Mediator.Send(new GetCertificatesQuery());
    }

    [HttpGet]
    [Route("{id:int}")]
    public async Task<FileResult> Index(Guid id)
    {
        var vm = await Mediator.Send(new ExportCertificatesQuery());

        return File(vm.Content, vm.ContentType, vm.FileName);
    }

    [HttpPost]
    [Route("")]
    public async Task<ActionResult<Guid>> Create([FromBody] CreateCertificateCommand command)
    {
        return await Mediator.Send(command);
    }

    [HttpDelete]
    [Route("{id:int}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        await Mediator.Send(new DeleteCertificateCommand { Id = id });

        return NoContent();
    }
}
