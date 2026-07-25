using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Stagehand.Reservations.Application.Abstractions.Persistence;
using Stagehand.Reservations.Infrastructure.Idempotency;
using Stagehand.Reservations.Infrastructure.Persistence;
using Stagehand.SharedKernel.Application.Idempotency;
using Stagehand.SharedKernel.Application.Persistence;

namespace Stagehand.Reservations.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddReservationsInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Reservations")
            ?? throw new InvalidOperationException(
                "Connection string 'Reservations' was not found in configuration.");

        services.AddDbContext<ReservationsDbContext>(options =>
            options.UseNpgsql(
                connectionString,
                npgsql => npgsql.MigrationsHistoryTable(
                    "__ef_migrations_history",
                    ReservationsDbContext.Schema)));

        services.AddScoped<IReservationRepository, ReservationRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IIdempotencyStore, IdempotencyStore>();

        return services;
    }
}
