using Stagehand.Catalog.Application.Listings.Search;
using Stagehand.Catalog.Domain.Listings;

namespace Stagehand.Catalog.Application.Abstractions.Persistence;

public interface IListingRepository
{
    Task<Listing?> GetByIdAsync(ListingId id, CancellationToken cancellationToken);

    Task<IReadOnlyList<Listing>> SearchAsync(
        ListingsCursor? cursor,
        int limit,
        ListingStatus? status,
        CancellationToken cancellationToken);

    void Add(Listing listing);
}
