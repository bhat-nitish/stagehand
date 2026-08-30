using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Stagehand.Reservations.Infrastructure.Messaging.Sagas;

namespace Stagehand.Reservations.Infrastructure.Persistence.Configurations;

internal sealed class ReservationStateConfiguration : IEntityTypeConfiguration<ReservationState>
{
    public void Configure(EntityTypeBuilder<ReservationState> builder)
    {
        builder.ToTable("reservation_state");

        builder.HasKey(s => s.CorrelationId);

        builder.Property(s => s.CorrelationId)
            .ValueGeneratedNever();

        builder.Property(s => s.CurrentState)
            .HasMaxLength(64)
            .IsRequired();

        builder.Property(s => s.ListingId)
            .IsRequired();

        builder.Property(s => s.Quantity)
            .IsRequired();

        builder.Property(s => s.ExpiresAt)
            .IsRequired();
    }
}
