using Microsoft.EntityFrameworkCore;
using Stagehand.Inventory.Application.Abstractions.Persistence;
using Stagehand.Inventory.Application.StockItems.Search;
using Stagehand.Inventory.Domain.StockItems;

namespace Stagehand.Inventory.Infrastructure.Persistence;

internal sealed class StockItemRepository : IStockItemRepository
{
    private readonly InventoryDbContext _dbContext;

    public StockItemRepository(InventoryDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<StockItem?> GetByIdAsync(StockItemId id, CancellationToken cancellationToken) =>
        _dbContext.StockItems
            .Include(s => s.Holds)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

    public Task<StockItem?> GetByListingAsync(Guid listingId, CancellationToken cancellationToken) =>
        _dbContext.StockItems
            .Include(s => s.Holds)
            .FirstOrDefaultAsync(s => s.ListingId == listingId, cancellationToken);

    public async Task<IReadOnlyList<StockItem>> SearchAsync(
        StockItemsCursor? cursor,
        int limit,
        StockItemStatus? status,
        CancellationToken cancellationToken)
    {
        var query = _dbContext.StockItems.AsNoTracking();

        if (status is not null)
        {
            var statusValue = status.Value;
            query = query.Where(s => s.Status == statusValue);
        }

        if (cursor is not null)
        {
            var id = cursor.Id;
            query = query.Where(s => s.Id > id);
        }

        return await query
            .OrderBy(s => s.Id)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    public void Add(StockItem stockItem) =>
        _dbContext.StockItems.Add(stockItem);
}
