namespace Stagehand.Contracts.Inventory;

public sealed record ReleaseStock(
    Guid ReservationId,
    Guid ListingId,
    string Reason);
