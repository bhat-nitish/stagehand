using Stagehand.Catalog.Domain.Listings;
using Stagehand.SharedKernel;

namespace Stagehand.Catalog.Domain.Listings.Events;

public sealed record ListingCreated(ListingId ListingId, string Title, DateTimeOffset StartsAt) : IDomainEvent
{
    public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
}