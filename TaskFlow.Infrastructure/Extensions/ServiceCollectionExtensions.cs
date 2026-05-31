using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using TaskFlow.Domain.Constants;
using TaskFlow.Domain.Entites;
using TaskFlow.Domain.Repositories;
using TaskFlow.Infrastructure.Persistence;
using TaskFlow.Infrastructure.Repositories;
using TaskFlow.Infrastructure.Seeders;

namespace TaskFlow.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("TaskFlowDB");
        services.AddDbContext<ApplicationDBContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddIdentityApiEndpoints<ApplicationUser>()
              .AddRoles<IdentityRole>()
              .AddEntityFrameworkStores<ApplicationDBContext>();

        services.AddAuthorization();

        services.AddScoped<ITaskRepository, TaskRepository>();
        services.AddScoped<IProjectRepository, ProjectRepository>();
        services.AddScoped<IOrganizationRepository, OrganizationRepository>();
        services.AddScoped<IRoleSeeder, RoleSeeder>();

        services.Configure<SmtpSettings>(configuration.GetSection("Smtp"));
        services.AddSingleton(sp => sp.GetRequiredService<IOptions<SmtpSettings>>().Value);
        services.AddScoped<IEmailSender, SmtpEmailSender>();
    }
}
