namespace Stagehand.Inventory.Application.StockItems.Create;

public sealed record CreateStockItemRequest(Guid ListingId, int TotalQuantity);
