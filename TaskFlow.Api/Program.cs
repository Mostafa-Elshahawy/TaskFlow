using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using TaskFlow.Api.Extensions;
using TaskFlow.Api.Infrastructure;
using TaskFlow.Application.Extensions;
using TaskFlow.Domain.Entites;
using TaskFlow.Infrastructure.Extensions;
using TaskFlow.Infrastructure.Persistence;
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

            await using var scope = app.Services.CreateAsyncScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();
            await db.Database.MigrateAsync();

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

            app.UseCors("Angular");

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
