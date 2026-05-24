using Microsoft.EntityFrameworkCore;
using Stagehand.Catalog.Application.Abstractions.Persistence;
using Stagehand.Catalog.Application.Listings.Search;
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

    public async Task<IReadOnlyList<Listing>> SearchAsync(
        ListingsCursor? cursor,
        int limit,
        ListingStatus? status,
        CancellationToken cancellationToken)
    {
        var query = _dbContext.Listings.AsNoTracking();

        if (status is not null)
        {
            var statusValue = status.Value;
            query = query.Where(l => l.Status == statusValue);
        }

        if (cursor is not null)
        {
            var startsAt = cursor.StartsAt;
            var id = cursor.Id;
            query = query.Where(l => l.StartsAt > startsAt || (l.StartsAt == startsAt && l.Id > id));
        }

        return await query
            .OrderBy(l => l.StartsAt)
            .ThenBy(l => l.Id)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    public void Add(Listing listing) =>
        _dbContext.Listings.Add(listing);
}
