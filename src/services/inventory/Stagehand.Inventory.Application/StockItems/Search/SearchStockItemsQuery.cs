using Stagehand.Inventory.Domain.StockItems;
using Stagehand.SharedKernel;
using Stagehand.SharedKernel.Application.Messaging;

namespace Stagehand.Inventory.Application.StockItems.Search;

public sealed record SearchStockItemsQuery(string? Cursor, int PageSize, StockItemStatus? Status)
    : IQuery<Result<SearchStockItemsResponse>>;
