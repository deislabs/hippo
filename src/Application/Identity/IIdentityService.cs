using Hippo.Application.Common.Models;

namespace Hippo.Application.Identity;

public interface IIdentityService
{
    Task<string> GetUserNameAsync(string id);

    Task<Account> FindByIdAsync(string id);

    Task<bool> IsInRoleAsync(string id, string role);

    Task<bool> AuthorizeAsync(string id, string policyName);

    Task<(Result Result, string UserId)> CreateUserAsync(string userName, string password);

    Task<Result> DeleteUserAsync(string id);

    Task<bool> CheckPasswordAsync(string userName, string password);
}
