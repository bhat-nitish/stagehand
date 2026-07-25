using Stagehand.SharedKernel;

namespace Stagehand.Reservations.Domain.Reservations.Events;

/// <summary>
/// Raised when a reservation is cancelled. <paramref name="PreviousStatus"/> tells downstream
/// handlers whether stock was already held: a cancellation from
/// <see cref="ReservationStatus.Confirmed"/> requires a compensating release in Inventory,
/// whereas one from <see cref="ReservationStatus.Pending"/> does not.
/// </summary>
public sealed record ReservationCancelled(
    ReservationId ReservationId,
    Guid ListingId,
    int Quantity,
    ReservationStatus PreviousStatus) : IDomainEvent
{
    public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
}
