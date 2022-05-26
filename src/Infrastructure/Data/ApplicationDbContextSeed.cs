using Hippo.Application.Common.Security;
using Hippo.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

namespace Hippo.Infrastructure.Data;

public static class ApplicationDbContextSeed
{
    public static async Task SeedIdentityRolesAsync(RoleManager<IdentityRole> roleManager)
    {
        var administratorRole = new IdentityRole(UserRole.Administrator);

        if (roleManager.Roles.All(r => r.Name != administratorRole.Name))
        {
            await roleManager.CreateAsync(administratorRole);
        }
    }

    public static async Task SeedAdministratorAccountsAsync(UserManager<Account> userManager, string username, string email, string password)
    {
        var user = new Account
        {
            UserName = username,
            Email = email
        };

        await userManager.CreateAsync(user, password);
        await userManager.AddToRoleAsync(user, UserRole.Administrator);
    }
}
