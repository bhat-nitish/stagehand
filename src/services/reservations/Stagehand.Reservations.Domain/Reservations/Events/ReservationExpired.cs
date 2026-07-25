using Stagehand.SharedKernel;

namespace Stagehand.Reservations.Domain.Reservations.Events;

public sealed record ReservationExpired(
    ReservationId ReservationId,
    Guid ListingId,
    int Quantity) : IDomainEvent
{
    public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
}
