using Stagehand.Reservations.Domain.Reservations;
using Stagehand.SharedKernel;

namespace Stagehand.Reservations.Application.Reservations;

public static class ReservationErrors
{
    public static ResultError NotFound(ReservationId id) => new(
        "Reservation.NotFound",
        $"The reservation with the identifier '{id.Value}' was not found.",
        ErrorType.NotFound);

    public static ResultError ConcurrencyConflict(ReservationId id) => new(
        "Reservation.ConcurrencyConflict",
        $"The reservation with the identifier '{id.Value}' was modified by another operation. Please retry.",
        ErrorType.Conflict);
}
