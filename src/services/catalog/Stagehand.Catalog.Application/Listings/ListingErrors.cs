using Stagehand.Catalog.Domain.Listings;
using Stagehand.SharedKernel;

namespace Stagehand.Catalog.Application.Listings;

public static class ListingErrors
{
    public static Error NotFound(ListingId id) => new(
        "Listing.NotFound",
        $"The listing with the identifier '{id.Value}' was not found.");
}
