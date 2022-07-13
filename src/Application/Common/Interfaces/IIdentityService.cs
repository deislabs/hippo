using Hippo.Application.Common.Models;

namespace Hippo.Application.Common.Interfaces;

public interface IIdentityService
{
    Task<string> GetUserIdAsync(string userName, CancellationToken cancellationToken);

    Task<string> GetUserNameAsync(string userId, CancellationToken cancellationToken);

    Task<(Result Result, string UserId)> CreateUserAsync(string userName, CancellationToken cancellationToken);

    Task<Result> DeleteUserAsync(string userId, CancellationToken cancellationToken);

    // authorization
    Task<bool> IsInRoleAsync(string userId, string role, CancellationToken cancellationToken);

    Task<bool> AuthorizeAsync(string userId, string policyName, CancellationToken cancellationToken);

    // email confirmation
    Task<Result> AddEmailAsync(string userId, string email, CancellationToken cancellationToken);

    Task<bool> IsEmailConfirmedAsync(string userId, CancellationToken cancellationToken);

    Task<string> GenerateEmailConfirmationTokenAsync(string userId, CancellationToken cancellationToken);

    Task<Result> ConfirmEmailAsync(string userId, string token, CancellationToken cancellationToken);

    // password auth
    Task<Result> AddPasswordAsync(string userId, string password, CancellationToken cancellationToken);

    Task<Result> ChangePasswordAsync(string userId, string oldPassword, string newPassword, CancellationToken cancellationToken);
}
