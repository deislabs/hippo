using Hippo.Web.Filters;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hippo.Web.Api;

[Area("API")]
[Route("api/[controller]")]
[ApiVersion("0.1")]
[ApiController]
[ApiExceptionFilter]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public abstract class ApiControllerBase : Controller
{
    private ISender _mediator = null!;

    protected ISender Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();
}
