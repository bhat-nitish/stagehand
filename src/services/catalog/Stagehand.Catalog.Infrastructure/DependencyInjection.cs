using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Stagehand.Catalog.Application.Abstractions.Persistence;
using Stagehand.Catalog.Infrastructure.Idempotency;
using Stagehand.Catalog.Infrastructure.Persistence;
using Stagehand.SharedKernel.Application.Idempotency;
using Stagehand.SharedKernel.Application.Persistence;

namespace Stagehand.Catalog.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddCatalogInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Catalog")
            ?? throw new InvalidOperationException(
                "Connection string 'Catalog' was not found in configuration.");

        services.AddDbContext<CatalogDbContext>(options =>
            options.UseNpgsql(
                connectionString,
                npgsql => npgsql.MigrationsHistoryTable(
                    "__ef_migrations_history",
                    CatalogDbContext.Schema)));

        services.AddScoped<IListingRepository, ListingRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IIdempotencyStore, IdempotencyStore>();

        return services;
    }
}
