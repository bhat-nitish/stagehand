using Stagehand.SharedKernel;

namespace Stagehand.Inventory.Domain.StockItems.Events;

public sealed record StockReleased(StockItemId StockItemId, int Quantity) : IDomainEvent
{
    public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
}
