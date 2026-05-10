using System;

namespace Stagehand.Catalog.Domain.Listings;

public readonly record struct ListingId(Guid Value)
{
    public static ListingId New() => new(Guid.NewGuid());
}
