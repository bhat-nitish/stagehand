using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Stagehand.Inventory.Domain.StockItems;

namespace Stagehand.Inventory.Infrastructure.Persistence.Configurations;

internal sealed class StockItemConfiguration : IEntityTypeConfiguration<StockItem>
{
    public void Configure(EntityTypeBuilder<StockItem> builder)
    {
        builder.ToTable("stock_items");

        builder.HasKey(s => s.Id);

        builder.Property<uint>("xmin").IsRowVersion();

        builder.Property(s => s.Id)
            .HasConversion(id => id.Value, value => new StockItemId(value))
            .ValueGeneratedNever();

        builder.Property(s => s.ListingId)
            .IsRequired();

        builder.HasIndex(s => s.ListingId)
            .IsUnique();

        builder.Property(s => s.TotalQuantity)
            .IsRequired();

        builder.Property(s => s.ReservedQuantity)
            .IsRequired();

        builder.Property(s => s.Status)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();

        builder.HasMany(s => s.Holds)
            .WithOne()
            .HasForeignKey("StockItemId")
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.Metadata
            .FindNavigation(nameof(StockItem.Holds))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.Ignore(s => s.AvailableQuantity);
        builder.Ignore(s => s.DomainEvents);
    }
}
