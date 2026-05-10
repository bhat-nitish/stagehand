using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Stagehand.SharedKernel.Application.Behaviours;

namespace Stagehand.SharedKernel.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddSharedKernelApplication(this IServiceCollection services)
    {

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehaviour<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));

        return services;
    }
}
