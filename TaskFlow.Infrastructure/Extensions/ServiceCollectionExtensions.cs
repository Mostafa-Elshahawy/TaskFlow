using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TaskFlow.Domain.Entites;
using TaskFlow.Infrastructure.Persistence;

namespace TaskFlow.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var ConnectionString = configuration.GetConnectionString("TaskFlowDB");
        services.AddDbContext<ApplicationDBContext>(options =>
            options.UseSqlServer(ConnectionString));

        services.AddIdentityApiEndpoints<ApplicationUser>()
              .AddRoles<IdentityRole>()
              .AddEntityFrameworkStores<ApplicationDBContext>();

        services.AddAuthorization();
    }
}
