using Microsoft.Extensions.DependencyInjection;

namespace TaskFlow.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddApplication(this IServiceCollection services)
    {
        var ApplicationAssembly = typeof(ServiceCollectionExtensions).Assembly;
    }
}
