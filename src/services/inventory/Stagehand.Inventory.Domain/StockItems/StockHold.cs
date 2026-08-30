namespace Stagehand.Inventory.Domain.StockItems;

public sealed class StockHold
{
    public Guid ReservationId { get; private set; }
    public int Quantity { get; private set; }
    public StockHoldStatus Status { get; private set; }

    private StockHold(Guid reservationId, int quantity)
    {
        ReservationId = reservationId;
        Quantity = quantity;
        Status = StockHoldStatus.Active;
    }

    internal static StockHold Create(Guid reservationId, int quantity) => new(reservationId, quantity);

    internal void Release() => Status = StockHoldStatus.Released;
}
