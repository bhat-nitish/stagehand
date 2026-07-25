using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Stagehand.Reservations.Infrastructure.Persistence;

internal sealed class ReservationsDbContextFactory : IDesignTimeDbContextFactory<ReservationsDbContext>
{
    public ReservationsDbContext CreateDbContext(string[] args)
    {
        // Design-time only (dotnet ef tooling). Never used at runtime — the app gets its
        // real connection string from Aspire/k8s via configuration. Not a secret.
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__Reservations")
            ?? "Host=localhost;Port=5432;Database=stagehand_reservations;Username=postgres;Password=postgres";

        var options = new DbContextOptionsBuilder<ReservationsDbContext>()
            .UseNpgsql(
                connectionString,
                npgsql => npgsql.MigrationsHistoryTable("__ef_migrations_history", ReservationsDbContext.Schema))
            .Options;

        return new ReservationsDbContext(options);
    }
}
