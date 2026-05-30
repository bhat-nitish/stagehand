namespace Stagehand.Catalog.Application.Listings.Create;

public sealed record CreateListingRequest(string Title, string Description, DateTimeOffset StartsAt);
