namespace Stagehand.Inventory.Application.StockItems.GetById;

public sealed record StockItemResponse(
    Guid Id,
    Guid ListingId,
    int TotalQuantity,
    int ReservedQuantity,
    int AvailableQuantity,
    string Status);
