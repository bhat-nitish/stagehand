using Stagehand.SharedKernel;

namespace Stagehand.Reservations.Domain.Reservations.Events;

public sealed record ReservationPlaced(
    ReservationId ReservationId,
    Guid ListingId,
    int Quantity,
    DateTimeOffset ExpiresAt) : IDomainEvent
{
    public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
}
