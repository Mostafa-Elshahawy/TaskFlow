using Microsoft.AspNetCore.Identity;
using TaskFlow.Domain.Constants;

namespace TaskFlow.Infrastructure.Seeders;

internal class RoleSeeder(RoleManager<IdentityRole> roleManager) : IRoleSeeder
{

    public async Task SeedRoles()
    {
        var roles = new[] { UserRoles.User, UserRoles.Admin, UserRoles.Manager };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }
}
