using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TaskFlow.Domain.Constants;
using TaskFlow.Domain.Entites;

namespace TaskFlow.Infrastructure.Seeders;

public sealed class IdentitySeeder(
    RoleManager<IdentityRole> roleManager,
    UserManager<ApplicationUser> userManager,
    IOptions<BootstrapAdminOptions> bootstrapOptions,
    ILogger<IdentitySeeder> logger) : IIdentitySeeder
{
    private static readonly string[] Roles = [UserRoles.User, UserRoles.Admin, UserRoles.Manager];

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        foreach (var role in Roles)
        {
            if (await roleManager.RoleExistsAsync(role))
                continue;

            EnsureSucceeded(
                await roleManager.CreateAsync(new IdentityRole(role)),
                $"create the '{role}' role");
        }

        var options = bootstrapOptions.Value;
        var hasEmail = !string.IsNullOrWhiteSpace(options.Email);
        var hasPassword = !string.IsNullOrWhiteSpace(options.Password);

        if (!hasEmail && !hasPassword)
        {
            logger.LogInformation("Bootstrap administrator credentials are not configured; user creation skipped.");
            return;
        }

        if (!hasEmail || !hasPassword)
            throw new InvalidOperationException(
                "Bootstrap administrator configuration must include both email and password.");

        var email = options.Email!.Trim();
        var administrator = await userManager.FindByEmailAsync(email);

        if (administrator is null)
        {
            administrator = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true
            };

            EnsureSucceeded(
                await userManager.CreateAsync(administrator, options.Password!),
                "create the bootstrap administrator");
        }
        else if (!administrator.EmailConfirmed)
        {
            administrator.EmailConfirmed = true;
            EnsureSucceeded(
                await userManager.UpdateAsync(administrator),
                "confirm the bootstrap administrator email");
        }

        if (!await userManager.IsInRoleAsync(administrator, UserRoles.Admin))
        {
            EnsureSucceeded(
                await userManager.AddToRoleAsync(administrator, UserRoles.Admin),
                "assign the bootstrap administrator role");
        }
    }

    private static void EnsureSucceeded(IdentityResult result, string operation)
    {
        if (result.Succeeded)
            return;

        var errors = string.Join("; ", result.Errors.Select(error => error.Description));
        throw new InvalidOperationException($"Unable to {operation}: {errors}");
    }
}
