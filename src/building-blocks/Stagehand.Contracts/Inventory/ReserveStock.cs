namespace Stagehand.Contracts.Inventory;

public sealed record ReserveStock(
    Guid ReservationId,
    Guid ListingId,
    int Quantity);
