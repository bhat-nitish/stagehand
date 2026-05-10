using Stagehand.Catalog.Domain.Listings;
using Stagehand.SharedKernel;
using Stagehand.SharedKernel.Application.Messaging;

namespace Stagehand.Catalog.Application.Listings.Create;

public sealed record CreateListingCommand(
    string Title,
    string Description,
    DateTimeOffset StartsAt) : ICommand<Result<ListingId>>;
