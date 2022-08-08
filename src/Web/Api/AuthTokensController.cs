using Hippo.Application.Accounts.Commands;
using Hippo.Application.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hippo.Web.Api;

public class AuthTokensController : ApiControllerBase
{
    [AllowAnonymous]
    [HttpPost]
    public async Task<ActionResult<TokenInfo>> CreateToken([FromBody] CreateTokenCommand command)
    {
        return await Mediator.Send(command);
    }
}
