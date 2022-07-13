using Hippo.Application.Accounts.Commands;
using Hippo.Application.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Hippo.Web.Api;

public class AccountController : ApiControllerBase
{
    [HttpPost]
    public async Task<ActionResult<string>> CreateAccount([FromBody] CreateAccountCommand command)
    {
        return await Mediator.Send(command);
    }

    [HttpPost("add_password")]
    public async Task<ActionResult> AddPassword([FromBody] AddPasswordCommand command)
    {
        await Mediator.Send(command);
        return NoContent();
    }

    [HttpPost("login_password")]
    [HttpPost("createtoken")]
    public async Task<ActionResult<TokenInfo>> LoginWithPassword([FromBody] LoginWithPasswordCommand command)
    {
        return await Mediator.Send(command);
    }

    [HttpPost("change_password")]
    public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordCommand command)
    {
        await Mediator.Send(command);
        return NoContent();
    }
}
