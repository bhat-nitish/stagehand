namespace Stagehand.Contracts.Inventory;

public sealed record StockReservationRejected(
    Guid ReservationId,
    Guid ListingId,
    string Reason);
