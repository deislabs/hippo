using Hippo.Application.Common.Interfaces;
using Hippo.Application.Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Hippo.Infrastructure.Identity;

public class IdentityService : IIdentityService
{
    private readonly UserManager<IdentityUser> _userManager;

    private readonly IUserClaimsPrincipalFactory<IdentityUser> _userClaimsPrincipalFactory;

    private readonly IAuthorizationService _authorizationService;

    public IdentityService(
            UserManager<IdentityUser> userManager,
            IUserClaimsPrincipalFactory<IdentityUser> userClaimsPrincipalFactory,
            IAuthorizationService authorizationService)
    {
        _userManager = userManager;
        _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
        _authorizationService = authorizationService;
    }

    private async Task<IdentityUser> getUserByIdAsync(string userId, CancellationToken cancellationToken)
    {
        return await _userManager.Users.SingleAsync(u => u.Id == userId, cancellationToken);
    }

    public async Task<string> GetUserIdAsync(string email, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(email);
        return user.Id;
    }

    public async Task<string> GetUserNameAsync(string userId, CancellationToken cancellationToken)
    {
        var user = await getUserByIdAsync(userId, cancellationToken);
        return user.UserName;
    }

    public async Task<(Result Result, string UserId)> CreateUserAsync(string userName, CancellationToken cancellationToken)
    {
        var user = new IdentityUser(userName);
        var result = await _userManager.CreateAsync(user);

        return (result.ToApplicationResult(), user.Id.ToString());
    }

    public async Task<bool> IsInRoleAsync(string userId, string role, CancellationToken cancellationToken)
    {
        var user = await getUserByIdAsync(userId, cancellationToken);

        return user is not null && await _userManager.IsInRoleAsync(user, role);
    }

    public async Task<bool> AuthorizeAsync(string userId, string policyName, CancellationToken cancellationToken)
    {
        var user = await _userManager.Users.SingleOrDefaultAsync(u => u.Id == userId, cancellationToken);
        if (user is null)
        {
            return false;
        }

        var principal = await _userClaimsPrincipalFactory.CreateAsync(user);

        var result = await _authorizationService.AuthorizeAsync(principal, policyName);

        return result.Succeeded;
    }

    public async Task<Result> AddEmailAsync(string userId, string email, CancellationToken cancellationToken)
    {
        var user = await getUserByIdAsync(userId, cancellationToken);
        var result = await _userManager.SetEmailAsync(user, email);
        return result.ToApplicationResult();
    }

    public async Task<bool> IsEmailConfirmedAsync(string userId, CancellationToken cancellationToken)
    {
        var user = await getUserByIdAsync(userId, cancellationToken);
        return user.EmailConfirmed;
    }

    public async Task<Result> DeleteUserAsync(string userId, CancellationToken cancellationToken)
    {
        var identityUser = await _userManager.Users.SingleOrDefaultAsync(u => u.Id == userId, cancellationToken);
        if (identityUser is null)
        {
            return Result.Success();
        }
        var result = await _userManager.DeleteAsync(identityUser);
        return result.ToApplicationResult();
    }

    private async Task<Result> DeleteUserAsync(IdentityUser user, CancellationToken cancellationToken)
    {
        var result = await _userManager.DeleteAsync(user);
        return result.ToApplicationResult();
    }

    public async Task<Result> AddPasswordAsync(string userId, string password, CancellationToken cancellationToken)
    {
        var user = await getUserByIdAsync(userId, cancellationToken);
        var result = await _userManager.AddPasswordAsync(user, password);
        return result.ToApplicationResult();
    }

    public async Task<string> GeneratePasswordResetTokenAsync(string userId, CancellationToken cancellationToken)
    {
        var user = await getUserByIdAsync(userId, cancellationToken);
        return await _userManager.GeneratePasswordResetTokenAsync(user);
    }

    public async Task<Result> ChangePasswordAsync(string userId, string oldPassword, string newPassword, CancellationToken cancellationToken)
    {
        var user = await getUserByIdAsync(userId, cancellationToken);

        var result = await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);
        return result.ToApplicationResult();
    }

    public async Task<string> GenerateEmailConfirmationTokenAsync(string userId, CancellationToken cancellationToken)
    {
        var user = await getUserByIdAsync(userId, cancellationToken);
        return await _userManager.GenerateEmailConfirmationTokenAsync(user);
    }

    public async Task<Result> ConfirmEmailAsync(string userId, string token, CancellationToken cancellationToken)
    {
        var user = await getUserByIdAsync(userId, cancellationToken);
        var result = await _userManager.ConfirmEmailAsync(user, token);
        return result.ToApplicationResult();
    }
}
