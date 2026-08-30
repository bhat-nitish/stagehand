using MassTransit;
using Microsoft.EntityFrameworkCore;
using Stagehand.Inventory.Domain.StockItems;

namespace Stagehand.Inventory.Infrastructure.Persistence;

public sealed class InventoryDbContext : DbContext
{
    public const string Schema = "inventory";

    public InventoryDbContext(DbContextOptions<InventoryDbContext> options) : base(options)
    {
    }

    public DbSet<StockItem> StockItems => Set<StockItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);

        modelBuilder.AddInboxStateEntity();
        modelBuilder.AddOutboxMessageEntity();
        modelBuilder.AddOutboxStateEntity();

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(InventoryDbContext).Assembly);
    }
}
