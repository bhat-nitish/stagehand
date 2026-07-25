namespace Stagehand.Reservations.Application.Reservations.GetById;

public sealed record ReservationResponse(
    Guid Id,
    Guid ListingId,
    int Quantity,
    string Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset ExpiresAt,
    string? RejectionReason);
