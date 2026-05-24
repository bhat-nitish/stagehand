using Stagehand.Catalog.Domain.Listings;
using Stagehand.SharedKernel;
using Stagehand.SharedKernel.Application.Messaging;

namespace Stagehand.Catalog.Application.Listings.Search;

public sealed record SearchListingsQuery(string? Cursor, int PageSize, ListingStatus? Status)
    : IQuery<Result<SearchListingsResponse>>;
