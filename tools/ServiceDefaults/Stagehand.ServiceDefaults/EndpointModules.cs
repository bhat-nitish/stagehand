using System.Reflection;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Stagehand.ServiceDefaults;

public interface IEndpointModule
{
    void MapEndpoints(IEndpointRouteBuilder app);
}

public static class EndpointModuleExtensions
{
    public static IServiceCollection AddEndpointModules(this IServiceCollection services, Assembly assembly)
    {
        var moduleTypes = assembly.GetTypes()
            .Where(type => type is { IsAbstract: false, IsInterface: false }
                && typeof(IEndpointModule).IsAssignableFrom(type));

        foreach (var type in moduleTypes)
        {
            services.AddSingleton(typeof(IEndpointModule), type);
        }

        return services;
    }

    public static IEndpointRouteBuilder MapEndpointModules(this IEndpointRouteBuilder app)
    {
        foreach (var module in app.ServiceProvider.GetServices<IEndpointModule>())
        {
            module.MapEndpoints(app);
        }

        return app;
    }
}
