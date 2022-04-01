using Hippo.Application.Identity;
using Microsoft.AspNetCore.Identity;

namespace Hippo.Infrastructure.Data;

public static class ApplicationDbContextSeed
{
    public static async Task SeedIdentityRolesAsync(UserManager<Account> userManager, RoleManager<IdentityRole> roleManager)
    {
        var administratorRole = new IdentityRole("Administrator");

        if (roleManager.Roles.All(r => r.Name != administratorRole.Name))
        {
            await roleManager.CreateAsync(administratorRole);
        }
    }
}
