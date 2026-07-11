using Stagehand.Inventory.Domain.StockItems;
using Stagehand.SharedKernel;
using Stagehand.SharedKernel.Application.Messaging;

namespace Stagehand.Inventory.Application.StockItems.Create;

public sealed record CreateStockItemCommand(Guid ListingId, int TotalQuantity)
    : ICommand<Result<StockItemId>>;
