using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Stagehand.Inventory.Infrastructure.Persistence;

internal sealed class InventoryDbContextFactory : IDesignTimeDbContextFactory<InventoryDbContext>
{
    public InventoryDbContext CreateDbContext(string[] args)
    {
        // Design-time only (dotnet ef tooling). Never used at runtime — the app gets its
        // real connection string from Aspire/k8s via configuration. Not a secret.
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__Inventory")
            ?? "Host=localhost;Port=5432;Database=stagehand_inventory;Username=postgres;Password=postgres";

        var options = new DbContextOptionsBuilder<InventoryDbContext>()
            .UseNpgsql(
                connectionString,
                npgsql => npgsql.MigrationsHistoryTable("__ef_migrations_history", InventoryDbContext.Schema))
            .Options;

        return new InventoryDbContext(options);
    }
}
