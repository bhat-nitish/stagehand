using Stagehand.SharedKernel;

namespace Stagehand.Inventory.Domain.StockItems.Events;

public sealed record StockItemCreated(StockItemId StockItemId, Guid ListingId, int TotalQuantity) : IDomainEvent
{
    public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
}
