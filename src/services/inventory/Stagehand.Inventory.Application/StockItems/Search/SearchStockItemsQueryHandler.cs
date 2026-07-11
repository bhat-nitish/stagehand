using Stagehand.Inventory.Application.Abstractions.Persistence;
using Stagehand.Inventory.Application.StockItems.GetById;
using Stagehand.SharedKernel;
using Stagehand.SharedKernel.Application.Messaging;

namespace Stagehand.Inventory.Application.StockItems.Search;

internal sealed class SearchStockItemsQueryHandler
    : IQueryHandler<SearchStockItemsQuery, Result<SearchStockItemsResponse>>
{
    private const int DefaultPageSize = 20;
    private const int MaxPageSize = 100;

    private readonly IStockItemRepository _stockItemRepository;

    public SearchStockItemsQueryHandler(IStockItemRepository stockItemRepository)
    {
        _stockItemRepository = stockItemRepository;
    }

    public async Task<Result<SearchStockItemsResponse>> Handle(
        SearchStockItemsQuery query,
        CancellationToken cancellationToken)
    {
        var pageSize = query.PageSize <= 0
            ? DefaultPageSize
            : Math.Min(query.PageSize, MaxPageSize);

        var cursor = StockItemsCursor.Decode(query.Cursor);

        var stockItems = await _stockItemRepository.SearchAsync(
            cursor,
            pageSize + 1,
            query.Status,
            cancellationToken);

        var hasMore = stockItems.Count > pageSize;

        var page = hasMore
            ? stockItems.Take(pageSize).ToList()
            : stockItems;

        var items = page
            .Select(stockItem => new StockItemResponse(
                stockItem.Id.Value,
                stockItem.ListingId,
                stockItem.TotalQuantity,
                stockItem.ReservedQuantity,
                stockItem.AvailableQuantity,
                stockItem.Status.ToString()))
            .ToList();

        var nextCursor = hasMore
            ? new StockItemsCursor(page[^1].Id).Encode()
            : null;

        return new SearchStockItemsResponse(items, nextCursor, hasMore);
    }
}
