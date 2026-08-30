using Stagehand.Reservations.Application.Reservations.GetById;

namespace Stagehand.Reservations.Application.Reservations.Search;

public sealed record SearchReservationsResponse(
    IReadOnlyList<ReservationResponse> Items,
    string? NextCursor,
    bool HasMore);
