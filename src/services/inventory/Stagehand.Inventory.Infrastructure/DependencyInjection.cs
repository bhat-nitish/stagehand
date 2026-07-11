using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Stagehand.Inventory.Application.Abstractions.Persistence;
using Stagehand.Inventory.Infrastructure.Idempotency;
using Stagehand.Inventory.Infrastructure.Persistence;
using Stagehand.SharedKernel.Application.Idempotency;
using Stagehand.SharedKernel.Application.Persistence;

namespace Stagehand.Inventory.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInventoryInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Inventory")
            ?? throw new InvalidOperationException(
                "Connection string 'Inventory' was not found in configuration.");

        services.AddDbContext<InventoryDbContext>(options =>
            options.UseNpgsql(
                connectionString,
                npgsql => npgsql.MigrationsHistoryTable(
                    "__ef_migrations_history",
                    InventoryDbContext.Schema)));

        services.AddScoped<IStockItemRepository, StockItemRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IIdempotencyStore, IdempotencyStore>();

        return services;
    }
}
