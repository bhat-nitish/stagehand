using Stagehand.Inventory.Domain.StockItems.Events;
using Stagehand.SharedKernel;

namespace Stagehand.Inventory.Domain.StockItems;

public sealed class StockItem : AggregateRoot<StockItemId>
{
    private readonly List<StockHold> _holds = new();

    public Guid ListingId { get; private set; }
    public int TotalQuantity { get; private set; }
    public int ReservedQuantity { get; private set; }
    public StockItemStatus Status { get; private set; }

    public IReadOnlyCollection<StockHold> Holds => _holds.AsReadOnly();

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

    public Result Reserve(Guid reservationId, int quantity)
    {
        // Already handled this reservation: succeed without raising an event, so a
        // redelivered message neither double-counts nor sends a second reply.
        if (_holds.Any(hold => hold.ReservationId == reservationId))
        {
            return Result.Success();
        }

        if (Status != StockItemStatus.Active)
        {
            return Result.Failure(StockItemErrors.NotActive(Id));
        }

        if (quantity > AvailableQuantity)
        {
            return Result.Failure(StockItemErrors.InsufficientStock(Id, quantity, AvailableQuantity));
        }

        _holds.Add(StockHold.Create(reservationId, quantity));
        ReservedQuantity += quantity;
        RaiseDomainEvent(new StockReserved(Id, ListingId, reservationId, quantity));
        return Result.Success();
    }

    public Result Release(Guid reservationId)
    {
        var hold = _holds.FirstOrDefault(h =>
            h.ReservationId == reservationId && h.Status == StockHoldStatus.Active);

        // No active hold: nothing to give back. Succeeding here is what makes a
        // duplicate release harmless.
        if (hold is null)
        {
            return Result.Success();
        }

        hold.Release();
        ReservedQuantity -= hold.Quantity;
        RaiseDomainEvent(new StockReleased(Id, ListingId, reservationId, hold.Quantity));
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
