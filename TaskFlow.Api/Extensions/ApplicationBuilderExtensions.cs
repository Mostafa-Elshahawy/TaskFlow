using Serilog;

namespace TaskFlow.Api.Extensions;

public static class ApplicationBuilderExtensions 
{
    public static void AddPresentation(this WebApplicationBuilder builder)
    {
        builder.Services.AddOpenApi();
        builder.Services.AddControllers();
        builder.Services.AddAuthentication();
        builder.Services.AddEndpointsApiExplorer();
        builder.Host.UseSerilog((context, configration) =>
            configration
                .ReadFrom.Configuration(context.Configuration)
        );
    }
}
