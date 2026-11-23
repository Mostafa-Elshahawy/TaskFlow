using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using TaskFlow.Domain.Constants;
using TaskFlow.Domain.Entites;
using TaskFlow.Infrastructure.Persistence;
using TaskFlow.Infrastructure.Seeders;
using Xunit;

namespace TaskFlow.Tests.Infrastructure;

public class IdentitySeederTests
{
    [Fact]
    public async Task SeedAsync_CreatesRolesAndConfiguredAdministrator()
    {
        await using var services = BuildServices(new BootstrapAdminOptions
        {
            Email = "admin@taskflow.test",
            Password = "Admin!23456"
        });

        var seeder = CreateSeeder(services);
        await seeder.SeedAsync(CancellationToken.None);

        var roles = services.GetRequiredService<RoleManager<IdentityRole>>();
        Assert.True(await roles.RoleExistsAsync(UserRoles.User));
        Assert.True(await roles.RoleExistsAsync(UserRoles.Admin));
        Assert.True(await roles.RoleExistsAsync(UserRoles.Manager));

        var users = services.GetRequiredService<UserManager<ApplicationUser>>();
        var administrator = await users.FindByEmailAsync("admin@taskflow.test");
        Assert.NotNull(administrator);
        Assert.True(await users.IsInRoleAsync(administrator, UserRoles.Admin));
        Assert.True(administrator.EmailConfirmed);
    }

    [Fact]
    public async Task SeedAsync_WhenRunTwice_DoesNotDuplicateAdministrator()
    {
        await using var services = BuildServices(new BootstrapAdminOptions
        {
            Email = "admin@taskflow.test",
            Password = "Admin!23456"
        });

        var seeder = CreateSeeder(services);
        await seeder.SeedAsync(CancellationToken.None);
        await seeder.SeedAsync(CancellationToken.None);

        var dbContext = services.GetRequiredService<ApplicationDBContext>();
        Assert.Equal(1, await dbContext.Users.CountAsync());
    }

    [Fact]
    public async Task SeedAsync_WithoutBootstrapCredentials_SeedsRolesWithoutCreatingUser()
    {
        await using var services = BuildServices(new BootstrapAdminOptions());

        var seeder = CreateSeeder(services);
        await seeder.SeedAsync(CancellationToken.None);

        var dbContext = services.GetRequiredService<ApplicationDBContext>();
        Assert.Equal(3, await dbContext.Roles.CountAsync());
        Assert.Empty(await dbContext.Users.ToListAsync());
    }

    private static ServiceProvider BuildServices(BootstrapAdminOptions options)
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddDbContext<ApplicationDBContext>(builder =>
            builder.UseInMemoryDatabase(Guid.NewGuid().ToString()));
        services
            .AddIdentityCore<ApplicationUser>()
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDBContext>();
        services.AddSingleton(Options.Create(options));
        return services.BuildServiceProvider();
    }

    private static IdentitySeeder CreateSeeder(IServiceProvider services)
    {
        return new IdentitySeeder(
            services.GetRequiredService<RoleManager<IdentityRole>>(),
            services.GetRequiredService<UserManager<ApplicationUser>>(),
            services.GetRequiredService<IOptions<BootstrapAdminOptions>>(),
            NullLogger<IdentitySeeder>.Instance);
    }
}
