using Stagehand.Catalog.Domain.Listings;

namespace Stagehand.Catalog.Application.Abstractions.Persistence;

public interface IListingRepository
{
    Task<Listing?> GetByIdAsync(ListingId id, CancellationToken cancellationToken);

    void Add(Listing listing);
}
