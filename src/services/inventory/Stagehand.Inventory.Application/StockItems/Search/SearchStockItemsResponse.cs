using Stagehand.Inventory.Application.StockItems.GetById;

namespace Stagehand.Inventory.Application.StockItems.Search;

public sealed record SearchStockItemsResponse(
    IReadOnlyList<StockItemResponse> Items,
    string? NextCursor,
    bool HasMore);
