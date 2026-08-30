using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Stagehand.Inventory.Domain.StockItems;

namespace Stagehand.Inventory.Infrastructure.Persistence.Configurations;

internal sealed class StockHoldConfiguration : IEntityTypeConfiguration<StockHold>
{
    public void Configure(EntityTypeBuilder<StockHold> builder)
    {
        builder.ToTable("stock_holds");

        builder.HasKey(h => h.ReservationId);

        builder.Property(h => h.ReservationId)
            .ValueGeneratedNever();

        builder.Property(h => h.Quantity)
            .IsRequired();

        builder.Property(h => h.Status)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();
    }
}
