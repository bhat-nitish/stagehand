using Stagehand.Reservations.Application.Reservations.Search;
using Stagehand.Reservations.Domain.Reservations;

namespace Stagehand.Reservations.Application.Abstractions.Persistence;

public interface IReservationRepository
{
    Task<Reservation?> GetByIdAsync(ReservationId id, CancellationToken cancellationToken);

    Task<IReadOnlyList<Reservation>> SearchAsync(
        ReservationsCursor? cursor,
        int limit,
        ReservationStatus? status,
        CancellationToken cancellationToken);

    void Add(Reservation reservation);
}
