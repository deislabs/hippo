using Hippo.Application.Accounts.Commands;
using Hippo.Application.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Hippo.Web.Api;

public class AccountController : ApiControllerBase
{
    [HttpPost]
    public async Task<ActionResult<string>> Register([FromBody] CreateAccountCommand command)
    {
        return await Mediator.Send(command);
    }

    [HttpPost("createtoken")]
    [HttpPost("login")]
    public async Task<ActionResult<ApiToken>> Login([FromBody] LoginAccountCommand command)
    {
        return await Mediator.Send(command);
    }

    public async Task<ActionResult<ApiToken>> RefreshToken([FromBody] RefreshCommand command)
    {
        return await Mediator.Send(command);
    }
}
