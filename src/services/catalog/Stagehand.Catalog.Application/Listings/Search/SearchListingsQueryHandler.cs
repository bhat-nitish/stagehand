using Stagehand.Catalog.Application.Abstractions.Persistence;
using Stagehand.Catalog.Application.Listings.GetById;
using Stagehand.SharedKernel;
using Stagehand.SharedKernel.Application.Messaging;

namespace Stagehand.Catalog.Application.Listings.Search;

internal sealed class SearchListingsQueryHandler
    : IQueryHandler<SearchListingsQuery, Result<SearchListingsResponse>>
{
    private const int DefaultPageSize = 20;
    private const int MaxPageSize = 100;

    private readonly IListingRepository _listingRepository;

    public SearchListingsQueryHandler(IListingRepository listingRepository)
    {
        _listingRepository = listingRepository;
    }

    public async Task<Result<SearchListingsResponse>> Handle(
        SearchListingsQuery query,
        CancellationToken cancellationToken)
    {
        var pageSize = query.PageSize <= 0
            ? DefaultPageSize
            : Math.Min(query.PageSize, MaxPageSize);

        var cursor = ListingsCursor.Decode(query.Cursor);

        var listings = await _listingRepository.SearchAsync(
            cursor,
            pageSize + 1,
            query.Status,
            cancellationToken);

        var hasMore = listings.Count > pageSize;

        var page = hasMore
            ? listings.Take(pageSize).ToList()
            : listings;

        var items = page
            .Select(listing => new ListingResponse(
                listing.Id.Value,
                listing.Title,
                listing.Description,
                listing.StartsAt,
                listing.Status.ToString()))
            .ToList();

        var nextCursor = hasMore
            ? new ListingsCursor(page[^1].StartsAt, page[^1].Id).Encode()
            : null;

        return new SearchListingsResponse(items, nextCursor, hasMore);
    }
}
