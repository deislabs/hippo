using Hippo.Application.Common.Models;
using Microsoft.AspNetCore.Identity;

namespace Hippo.Infrastructure.Identity;

public static class SignInResultExtensions
{
    public static Result ToApplicationResult(this SignInResult result)
    {
        if (result.Succeeded)
        {
            return Result.Success();
        }

        var errors = new string[] { };

        if (result.IsLockedOut)
        {
            errors.Append("Account locked.");
        }

        if (result.IsNotAllowed)
        {
            errors.Append("Account not allowed to sign in.");
        }

        if (result.RequiresTwoFactor)
        {
            errors.Append("Account requires two-factor authentication to log in.");
        }

        return Result.Failure(errors);
    }
}
