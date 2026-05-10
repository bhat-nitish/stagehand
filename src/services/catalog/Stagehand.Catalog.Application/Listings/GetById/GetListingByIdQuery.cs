using Stagehand.Catalog.Domain.Listings;
using Stagehand.SharedKernel;
using Stagehand.SharedKernel.Application.Messaging;

namespace Stagehand.Catalog.Application.Listings.GetById;

public sealed record GetListingByIdQuery(ListingId ListingId) : IQuery<Result<ListingResponse>>;
