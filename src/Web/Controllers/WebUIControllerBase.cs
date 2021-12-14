using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Hippo.Web.Controllers;

public abstract class WebUIControllerBase : Controller
{
    private ISender _mediator = null!;

    protected ISender Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();
}
