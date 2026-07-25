using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Stagehand.Reservations.Domain.Reservations;

namespace Stagehand.Reservations.Infrastructure.Persistence.Configurations;

internal sealed class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
{
    public void Configure(EntityTypeBuilder<Reservation> builder)
    {
        builder.ToTable("reservations");

        builder.HasKey(r => r.Id);

        builder.Property<uint>("xmin").IsRowVersion();

        builder.Property(r => r.Id)
            .HasConversion(id => id.Value, value => new ReservationId(value))
            .ValueGeneratedNever();

        builder.Property(r => r.ListingId)
            .IsRequired();

        builder.Property(r => r.Quantity)
            .IsRequired();

        builder.Property(r => r.Status)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(r => r.CreatedAt)
            .IsRequired();

        builder.Property(r => r.ExpiresAt)
            .IsRequired();

        // Populated from Inventory's rejection over the bus — bounded so another
        // service cannot drive an unbounded write into this table.
        builder.Property(r => r.RejectionReason)
            .HasMaxLength(256);

        // Backs the sole ordering path of the search endpoint: the (CreatedAt, Id) keyset cursor.
        builder.HasIndex(r => new { r.CreatedAt, r.Id });

        builder.Ignore(r => r.DomainEvents);
    }
}
