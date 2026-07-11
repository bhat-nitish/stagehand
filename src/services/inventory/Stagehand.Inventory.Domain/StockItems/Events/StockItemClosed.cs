using Stagehand.SharedKernel;

namespace Stagehand.Inventory.Domain.StockItems.Events;

public sealed record StockItemClosed(StockItemId StockItemId) : IDomainEvent
{
    public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
}
