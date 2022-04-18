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
    public async Task<ActionResult<CertificatesVm>> Index()
    {
        return await Mediator.Send(new GetCertificatesQuery());
    }

    [HttpGet("export")]
    public async Task<FileResult> Export()
    {
        var vm = await Mediator.Send(new ExportCertificatesQuery());

        return File(vm.Content, vm.ContentType, vm.FileName);
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> Create([FromBody] CreateCertificateCommand command)
    {
        return await Mediator.Send(command);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(Guid id, UpdateCertificateCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest();
        }

        await Mediator.Send(command);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        await Mediator.Send(new DeleteCertificateCommand { Id = id });

        return NoContent();
    }
}
