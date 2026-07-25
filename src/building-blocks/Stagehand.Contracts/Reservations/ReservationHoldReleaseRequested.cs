namespace Stagehand.Contracts.Reservations;

/// <summary>
/// Published by Reservations when stock held for a reservation must be released.
/// Covers EVERY compensation path — cancellation, expiry, and a confirmation that
/// arrived too late to apply.
/// </summary>
/// <remarks>
/// WIRE CONTRACT: the full type name is the exchange name. See <see cref="ReservationRequested"/>.
///
/// ONE message rather than one per cause, because Reservations cannot always know whether a
/// hold exists. A Pending reservation means "no confirmation received", NOT "no stock held" —
/// Inventory may have reserved already with the reply still in flight. So Reservations
/// publishes this whenever a hold MIGHT exist, and Inventory's release is idempotent:
/// no active hold for <see cref="ReservationId"/> means no-op.
///
/// <see cref="Reason"/> is for audit and metrics ONLY. Never branch consumer behaviour on it —
/// the release path must stay identical regardless of cause.
/// </remarks>
public sealed record ReservationHoldReleaseRequested(
    Guid ReservationId,
    Guid ListingId,
    int Quantity,
    string Reason);
