namespace Stagehand.Inventory.Domain.StockItems;

public readonly record struct StockItemId(Guid Value) : IComparable<StockItemId>
{
    public static StockItemId New() => new(Guid.NewGuid());

    public int CompareTo(StockItemId other) => Value.CompareTo(other.Value);

    public static bool operator <(StockItemId left, StockItemId right) => left.CompareTo(right) < 0;

    public static bool operator >(StockItemId left, StockItemId right) => left.CompareTo(right) > 0;

    public static bool operator <=(StockItemId left, StockItemId right) => left.CompareTo(right) <= 0;

    public static bool operator >=(StockItemId left, StockItemId right) => left.CompareTo(right) >= 0;
}
