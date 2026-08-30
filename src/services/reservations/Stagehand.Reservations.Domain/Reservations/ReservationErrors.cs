using Stagehand.SharedKernel;

namespace Stagehand.Reservations.Domain.Reservations;

public static class ReservationErrors
{
    public static ResultError NotPending(ReservationId id, ReservationStatus status) => new(
        "Reservation.NotPending",
        $"The reservation with the identifier '{id.Value}' is '{status}' and is no longer pending.",
        ErrorType.Conflict);

    public static ResultError CannotCancel(ReservationId id, ReservationStatus status) => new(
        "Reservation.CannotCancel",
        $"The reservation with the identifier '{id.Value}' is '{status}' and can no longer be cancelled.",
        ErrorType.Conflict);
}
