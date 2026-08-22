using Stagehand.Inventory.Application.StockItems.Search;
using Stagehand.Inventory.Domain.StockItems;

namespace Stagehand.Inventory.Application.Abstractions.Persistence;

public interface IStockItemRepository
{
    Task<StockItem?> GetByIdAsync(StockItemId id, CancellationToken cancellationToken);

    Task<StockItem?> GetByListingAsync(Guid listingId, CancellationToken cancellationToken);

    Task<IReadOnlyList<StockItem>> SearchAsync(
        StockItemsCursor? cursor,
        int limit,
        StockItemStatus? status,
        CancellationToken cancellationToken);

    void Add(StockItem stockItem);
}
