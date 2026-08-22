using Stagehand.Inventory.Domain.StockItems;
using Stagehand.SharedKernel;
using Stagehand.SharedKernel.Application.Messaging;

namespace Stagehand.Inventory.Application.StockItems.Reserve;

public sealed record ReserveStockCommand(StockItemId StockItemId, Guid ReservationId, int Quantity)
    : ICommand<Result>;
