using Stagehand.Inventory.Domain.StockItems.Events;
using Stagehand.SharedKernel;

namespace Stagehand.Inventory.Domain.StockItems;

public sealed class StockItem : AggregateRoot<StockItemId>
{
    public Guid ListingId { get; private set; }
    public int TotalQuantity { get; private set; }
    public int ReservedQuantity { get; private set; }
    public StockItemStatus Status { get; private set; }

    public int AvailableQuantity => TotalQuantity - ReservedQuantity;

    private StockItem(StockItemId id, Guid listingId, int totalQuantity)
        : base(id)
    {
        ListingId = listingId;
        TotalQuantity = totalQuantity;
        ReservedQuantity = 0;
        Status = StockItemStatus.Active;
    }

    public static StockItem Create(Guid listingId, int totalQuantity)
    {
        var stockItem = new StockItem(StockItemId.New(), listingId, totalQuantity);
        stockItem.RaiseDomainEvent(new StockItemCreated(stockItem.Id, listingId, totalQuantity));
        return stockItem;
    }

    public Result Reserve(int quantity)
    {
        if (Status != StockItemStatus.Active)
        {
            return Result.Failure(StockItemErrors.NotActive(Id));
        }

        if (quantity > AvailableQuantity)
        {
            return Result.Failure(StockItemErrors.InsufficientStock(Id, quantity, AvailableQuantity));
        }

        ReservedQuantity += quantity;
        RaiseDomainEvent(new StockReserved(Id, quantity));
        return Result.Success();
    }

    public Result Release(int quantity)
    {
        if (quantity > ReservedQuantity)
        {
            return Result.Failure(StockItemErrors.ReleaseExceedsReserved(Id, quantity, ReservedQuantity));
        }

        ReservedQuantity -= quantity;
        RaiseDomainEvent(new StockReleased(Id, quantity));
        return Result.Success();
    }

    public void Close()
    {
        if (Status == StockItemStatus.Closed)
        {
            return;
        }

        Status = StockItemStatus.Closed;
        RaiseDomainEvent(new StockItemClosed(Id));
    }
}
