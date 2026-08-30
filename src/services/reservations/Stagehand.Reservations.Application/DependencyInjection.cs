using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Stagehand.SharedKernel.Application;

namespace Stagehand.Reservations.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddReservationsApplication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSharedKernelApplication();

        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        services.Configure<ReservationOptions>(
            configuration.GetSection(ReservationOptions.SectionName));

        services.AddSingleton(TimeProvider.System);

        return services;
    }
}
