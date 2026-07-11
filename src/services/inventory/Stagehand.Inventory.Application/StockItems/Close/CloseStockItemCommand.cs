using Stagehand.Inventory.Domain.StockItems;
using Stagehand.SharedKernel;
using Stagehand.SharedKernel.Application.Messaging;

namespace Stagehand.Inventory.Application.StockItems.Close;

public sealed record CloseStockItemCommand(StockItemId StockItemId) : ICommand<Result>;
