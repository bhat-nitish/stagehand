namespace Stagehand.Contracts.Inventory;

/// <summary>
/// Published by Inventory when stock has been successfully held for a reservation.
/// Reservations confirms the reservation in response.
/// </summary>
/// <remarks>
/// WIRE CONTRACT: the full type name is the exchange name.
///
/// May arrive after the reservation has left Pending (cancelled or expired in the meantime).
/// In that case Confirm() fails with NotPending and Reservations must publish
/// ReservationHoldReleaseRequested to give the stock back — otherwise it leaks.
/// </remarks>
public sealed record StockReserved(
    Guid ReservationId,
    Guid ListingId,
    int Quantity);
