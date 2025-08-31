using Scalar.AspNetCore;
using System.Threading.Tasks;
using TaskFlow.Api.Extensions;
using TaskFlow.Api.Infrastructure;
using TaskFlow.Application.Extensions;
using TaskFlow.Domain.Entites;
using TaskFlow.Infrastructure.Extensions;
using TaskFlow.Infrastructure.Seeders;

namespace TaskFlow
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.AddPresentation();
            builder.Services.AddApplication();
            builder.Services.AddInfrastructure(builder.Configuration);

            builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
            builder.Services.AddProblemDetails();

            var app = builder.Build();

            var scope = app.Services.CreateScope();
            var seeder = scope.ServiceProvider.GetRequiredService<IRoleSeeder>();

            await seeder.SeedRoles();

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.MapScalarApiReference(options =>
                {
                    options.
                       WithTitle("TaskFlow API").
                       WithTheme(ScalarTheme.DeepSpace);
                });
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseExceptionHandler();

            app.MapGroup("/api/identity")
               .WithTags("Identity")
               .MapIdentityApi<ApplicationUser>();

            app.MapControllers();

            app.Run();
        }
    }
}
