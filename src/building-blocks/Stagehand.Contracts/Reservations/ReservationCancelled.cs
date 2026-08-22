namespace Stagehand.Contracts.Reservations;

public sealed record ReservationCancelled(
    Guid ReservationId,
    Guid ListingId,
    int Quantity);
