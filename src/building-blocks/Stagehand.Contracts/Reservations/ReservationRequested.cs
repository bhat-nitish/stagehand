namespace Stagehand.Contracts.Reservations;

public sealed record ReservationRequested(
    Guid ReservationId,
    Guid ListingId,
    int Quantity,
    DateTimeOffset ExpiresAt);
