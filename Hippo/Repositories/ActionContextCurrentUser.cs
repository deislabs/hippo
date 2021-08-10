using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Hippo.Repositories
{
    public class ActionContextCurrentUser : ICurrentUser
    {
        private readonly IActionContextAccessor _actionContext;

        public ActionContextCurrentUser(IActionContextAccessor actionContext)
        {
            _actionContext = actionContext;
        }

        public string Name() => _actionContext.ActionContext.HttpContext.User.Identity.Name;
    }
}
