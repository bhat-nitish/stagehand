namespace Stagehand.Contracts.Inventory;

/// <summary>
/// Published by Inventory when stock could NOT be held for a reservation — insufficient
/// stock, the stock item is closed, or no stock item exists for the listing.
/// Reservations rejects the reservation in response.
/// </summary>
/// <remarks>
/// WIRE CONTRACT: the full type name is the exchange name.
///
/// <see cref="Reason"/> is surfaced to the caller via Reservation.RejectionReason, which is
/// capped at 256 characters in the Reservations schema — keep it short.
/// </remarks>
public sealed record StockReservationRejected(
    Guid ReservationId,
    Guid ListingId,
    string Reason);
