using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Stagehand.Catalog.Infrastructure.Persistence;

internal sealed class CatalogDbContextFactory : IDesignTimeDbContextFactory<CatalogDbContext>
{
    public CatalogDbContext CreateDbContext(string[] args)
    {
        // Design-time only (dotnet ef tooling). Never used at runtime — the app gets its
        // real connection string from Aspire/k8s via configuration. Not a secret.
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__Catalog")
            ?? "Host=localhost;Port=5432;Database=stagehand_catalog;Username=postgres;Password=postgres";

        var options = new DbContextOptionsBuilder<CatalogDbContext>()
            .UseNpgsql(
                connectionString,
                npgsql => npgsql.MigrationsHistoryTable("__ef_migrations_history", CatalogDbContext.Schema))
            .Options;

        return new CatalogDbContext(options);
    }
}
