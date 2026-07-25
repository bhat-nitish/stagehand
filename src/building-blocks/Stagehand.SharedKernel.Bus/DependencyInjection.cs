using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Stagehand.SharedKernel.Bus;

public static class DependencyInjection
{
    private const string ConnectionStringName = "rabbitmq";

    /// <summary>
    /// Registers the MassTransit bus over RabbitMQ. Commands and queries travel
    /// in-process through the mediator; integration events travel across services
    /// through this bus.
    /// </summary>
    /// <param name="configure">
    /// Hook for each service to register its own consumers, sagas and outbox.
    /// </param>
    public static IServiceCollection AddStagehandMessaging(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<IBusRegistrationConfigurator>? configure = null)
    {
        var connectionString = configuration.GetConnectionString(ConnectionStringName)
            ?? throw new InvalidOperationException(
                $"Connection string '{ConnectionStringName}' was not found.");

        services.AddMassTransit(busConfigurator =>
        {
            busConfigurator.SetKebabCaseEndpointNameFormatter();

            configure?.Invoke(busConfigurator);

            busConfigurator.UsingRabbitMq((context, rabbit) =>
            {
                rabbit.Host(new Uri(connectionString));

                rabbit.UseMessageRetry(retry =>
                    retry.Intervals(200, 500, 1000, 3000, 5000));

                rabbit.ConfigureEndpoints(context);
            });
        });

        return services;
    }
}
