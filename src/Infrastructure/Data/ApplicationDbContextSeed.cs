using Hippo.Application.Common.Security;
using Hippo.Core.Entities;
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

    public static async Task SeedAdministratorAccountsAsync(UserManager<IdentityUser> userManager, string username, string password)
    {
        var identityUser = new IdentityUser(username);
        await userManager.CreateAsync(identityUser, password);
        await userManager.AddToRoleAsync(identityUser, UserRole.Administrator);
    }
}
