using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Stagehand.SharedKernel.Application;

namespace Stagehand.Inventory.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddInventoryApplication(this IServiceCollection services)
    {
        services.AddSharedKernelApplication();

        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        return services;
    }
}
