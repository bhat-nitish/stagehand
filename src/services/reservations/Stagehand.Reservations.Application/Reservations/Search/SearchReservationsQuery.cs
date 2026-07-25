using Stagehand.Reservations.Domain.Reservations;
using Stagehand.SharedKernel;
using Stagehand.SharedKernel.Application.Messaging;

namespace Stagehand.Reservations.Application.Reservations.Search;

public sealed record SearchReservationsQuery(string? Cursor, int PageSize, ReservationStatus? Status)
    : IQuery<Result<SearchReservationsResponse>>;
