using Microsoft.EntityFrameworkCore;
using Stagehand.Catalog.Application.Abstractions.Persistence;
using Stagehand.Catalog.Domain.Listings;

namespace Stagehand.Catalog.Infrastructure.Persistence;

internal sealed class ListingRepository : IListingRepository
{
    private readonly CatalogDbContext _dbContext;

    public ListingRepository(CatalogDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Listing?> GetByIdAsync(ListingId id, CancellationToken cancellationToken) =>
        _dbContext.Listings.FirstOrDefaultAsync(l => l.Id == id, cancellationToken);

    public void Add(Listing listing) =>
        _dbContext.Listings.Add(listing);
}
