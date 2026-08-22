using Stagehand.SharedKernel;

namespace Stagehand.Inventory.Domain.StockItems;

public static class StockItemErrors
{
    public static ResultError NotActive(StockItemId id) => new(
        "StockItem.NotActive",
        $"The stock item with the identifier '{id.Value}' is not active and cannot be reserved against.",
        ErrorType.Conflict);

    public static ResultError InsufficientStock(StockItemId id, int requested, int available) => new(
        "StockItem.InsufficientStock",
        $"Cannot reserve {requested} unit(s); only {available} available for stock item '{id.Value}'.",
        ErrorType.Conflict);
}
