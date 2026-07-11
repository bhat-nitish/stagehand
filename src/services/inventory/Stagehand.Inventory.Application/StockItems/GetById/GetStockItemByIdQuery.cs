using Stagehand.Inventory.Domain.StockItems;
using Stagehand.SharedKernel;
using Stagehand.SharedKernel.Application.Messaging;

namespace Stagehand.Inventory.Application.StockItems.GetById;

public sealed record GetStockItemByIdQuery(StockItemId StockItemId) : IQuery<Result<StockItemResponse>>;
