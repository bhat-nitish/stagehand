using Stagehand.Catalog.Domain.Listings;
using Stagehand.SharedKernel;
using Stagehand.SharedKernel.Application.Messaging;

namespace Stagehand.Catalog.Application.Listings.Cancel;

public sealed record CancelListingCommand(ListingId ListingId, string Reason) : ICommand<Result>;
