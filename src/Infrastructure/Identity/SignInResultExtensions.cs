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

        var errors = new List<string>();

        if (result.IsLockedOut)
        {
            errors.Add("Account locked.");
        }

        if (result.IsNotAllowed)
        {
            errors.Add("Account not allowed to sign in.");
        }

        if (result.RequiresTwoFactor)
        {
            errors.Add("Account requires two-factor authentication to log in.");
        }

        if (!result.RequiresTwoFactor &&
            !result.IsNotAllowed &&
            !result.IsLockedOut)
        {
            errors.Add("Credentials are not valid.");
        }

        return Result.Failure(errors);
    }
}
