namespace Stagehand.Catalog.Application.Listings.GetById;

public sealed record ListingResponse(
    Guid Id,
    string Title,
    string Description,
    DateTimeOffset StartsAt,
    string Status);
