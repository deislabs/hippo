using Hippo.Application.Accounts.Commands;
using Hippo.Application.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Hippo.Web.Api;

[Route("api/account/[action]")]
[ApiController]
public class AccountApiController : ApiControllerBase
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
