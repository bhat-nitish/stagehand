using Stagehand.Inventory.Domain.StockItems;
using Stagehand.SharedKernel;

namespace Stagehand.Inventory.Application.StockItems;

public static class StockItemErrors
{
    public static ResultError NotFound(StockItemId id) => new(
        "StockItem.NotFound",
        $"The stock item with the identifier '{id.Value}' was not found.",
        ErrorType.NotFound);

    public static ResultError NoStockItemForListing(Guid listingId) => new(
        "StockItem.NoStockItemForListing",
        $"No stock item exists for the listing '{listingId}'.",
        ErrorType.NotFound);

    public static ResultError AlreadyExistsForListing(Guid listingId) => new(
        "StockItem.AlreadyExistsForListing",
        $"A stock item already exists for the listing '{listingId}'.",
        ErrorType.Conflict);

    public static ResultError ConcurrencyConflict(StockItemId id) => new(
        "StockItem.ConcurrencyConflict",
        $"The stock item with the identifier '{id.Value}' was modified by another operation. Please retry.",
        ErrorType.Conflict);
}
