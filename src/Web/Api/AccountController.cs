using Hippo.Application.Accounts.Commands;
using Hippo.Application.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hippo.Web.Api;

public class AccountController : ApiControllerBase
{
    [AllowAnonymous]
    [HttpPost]
    public async Task<ActionResult<string>> Register([FromBody] CreateAccountCommand command)
    {
        return await Mediator.Send(command);
    }

    [AllowAnonymous]
    [HttpPost("createtoken")]
    public async Task<ActionResult<TokenInfo>> CreateToken([FromBody] CreateTokenCommand command)
    {
        return await Mediator.Send(command);
    }
}
