using Stagehand.Catalog.Domain.Listings;
using Stagehand.SharedKernel;

namespace Stagehand.Catalog.Application.Listings;

public static class ListingErrors
{
    public static ResultError NotFound(ListingId id) => new(
        "Listing.NotFound",
        $"The listing with the identifier '{id.Value}' was not found.",
        ErrorType.NotFound);

    public static ResultError ConcurrencyConflict(ListingId id) => new(
        "Listing.ConcurrencyConflict",
        $"The listing with the identifier '{id.Value}' was modified by another operation. Please retry.",
        ErrorType.Conflict);
}
