namespace Stagehand.Contracts.Reservations;

/// <summary>
/// Published by Reservations when a reservation is placed. Inventory holds stock in response.
/// </summary>
/// <remarks>
/// WIRE CONTRACT: MassTransit derives the RabbitMQ exchange name from this type's FULL name
/// (namespace + type name). Renaming or moving it is a breaking change for any deployed
/// consumer — treat it like a published API, not an internal type.
///
/// Primitives only, by design. No ReservationId/ReservationStatus — those are Reservations'
/// domain types and must not cross the context boundary.
/// </remarks>
public sealed record ReservationRequested(
    Guid ReservationId,
    Guid ListingId,
    int Quantity,
    DateTimeOffset ExpiresAt);
