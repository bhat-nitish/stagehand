namespace Stagehand.Inventory.Application.StockItems.Reserve;

public sealed record ReserveStockRequest(Guid ReservationId, int Quantity);
