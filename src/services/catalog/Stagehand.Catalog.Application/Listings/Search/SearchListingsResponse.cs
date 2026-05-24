using Stagehand.Catalog.Application.Listings.GetById;

namespace Stagehand.Catalog.Application.Listings.Search;

public sealed record SearchListingsResponse(
    IReadOnlyList<ListingResponse> Items,
    string? NextCursor,
    bool HasMore);
