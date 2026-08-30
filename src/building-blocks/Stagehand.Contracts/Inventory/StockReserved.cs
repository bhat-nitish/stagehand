namespace Stagehand.Contracts.Inventory;

public sealed record StockReserved(
    Guid ReservationId,
    Guid ListingId,
    int Quantity);
