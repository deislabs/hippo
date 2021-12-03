using Hippo.Application.Accounts.Commands;
using Hippo.Application.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Hippo.Web.Api;

[Route("api/[controller]", Name = "Api[controller]")]
[ApiController]
public class AccountController : ApiControllerBase
{
    [HttpPost]
    public async Task<ActionResult<string>> Register(CreateAccountCommand command)
    {
        return await Mediator.Send(command);
    }

    [HttpPost]
    public async Task<ActionResult<TokenInfo>> CreateToken(CreateTokenCommand command)
    {
        return await Mediator.Send(command);
    }
}
