using Hippo.Application.Common.Interfaces;
using Hippo.Application.Common.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Hippo.Infrastructure.Identity;

public class SignInService : ISignInService
{
    private readonly SignInManager<IdentityUser> _signInManager;

    private readonly UserManager<IdentityUser> _userManager;

    public SignInService(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    public async Task<Result> PasswordSignInAsync(string userId, string password, bool rememberMe = false)
    {
        var user = _userManager.Users.SingleOrDefault(u => u.Id == userId);
        if (user is null)
        {
            return Result.Failure(new string[] { "Account does not exist." });
        }

        var result = await _signInManager.PasswordSignInAsync(user, password, rememberMe, false);

        return result.ToApplicationResult();
    }

    public async Task<Unit> SignOutAsync()
    {
        await _signInManager.SignOutAsync();
        return Unit.Value;
    }
}
