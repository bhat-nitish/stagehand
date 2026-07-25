using Stagehand.SharedKernel;

namespace Stagehand.Reservations.Domain.Reservations.Events;

public sealed record ReservationConfirmed(
    ReservationId ReservationId,
    Guid ListingId,
    int Quantity) : IDomainEvent
{
    public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
}
