using Stagehand.SharedKernel;

namespace Stagehand.Reservations.Domain.Reservations.Events;

public sealed record ReservationRejected(
    ReservationId ReservationId,
    Guid ListingId,
    string Reason) : IDomainEvent
{
    public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
}
