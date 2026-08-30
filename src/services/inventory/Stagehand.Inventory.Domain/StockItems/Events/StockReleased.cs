using Stagehand.SharedKernel;

namespace Stagehand.Inventory.Domain.StockItems.Events;

public sealed record StockReleased(
    StockItemId StockItemId,
    Guid ListingId,
    Guid ReservationId,
    int Quantity) : IDomainEvent
{
    public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
}
