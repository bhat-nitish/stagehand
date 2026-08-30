using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Stagehand.Reservations.Infrastructure.Idempotency;

namespace Stagehand.Reservations.Infrastructure.Persistence.Configurations;

internal sealed class IdempotentRequestConfiguration : IEntityTypeConfiguration<IdempotentRequest>
{
    public void Configure(EntityTypeBuilder<IdempotentRequest> builder)
    {
        builder.ToTable("idempotency_requests");

        builder.HasKey(r => r.Key);

        builder.Property(r => r.Key)
            .HasMaxLength(100);

        builder.Property(r => r.RequestHash)
            .IsRequired()
            .HasMaxLength(128);

        builder.Property(r => r.StatusCode);

        builder.Property(r => r.ResponseBody);

        builder.Property(r => r.CreatedAtUtc)
            .IsRequired();
    }
}
