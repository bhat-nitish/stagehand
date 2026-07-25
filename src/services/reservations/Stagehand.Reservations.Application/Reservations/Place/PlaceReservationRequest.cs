namespace Stagehand.Reservations.Application.Reservations.Place;

public sealed record PlaceReservationRequest(Guid ListingId, int Quantity);
