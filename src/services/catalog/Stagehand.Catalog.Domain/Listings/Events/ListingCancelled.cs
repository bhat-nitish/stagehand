using Stagehand.SharedKernel;

namespace Stagehand.Catalog.Domain.Listings.Events;

public sealed record ListingCancelled(ListingId ListingId, string Reason) : IDomainEvent
{
    public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
}
