using Stagehand.SharedKernel;

namespace Stagehand.Reservations.Domain.Reservations.Events;

public sealed record ReservationCancelled(
    ReservationId ReservationId,
    Guid ListingId,
    int Quantity) : IDomainEvent
{
    public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
}
