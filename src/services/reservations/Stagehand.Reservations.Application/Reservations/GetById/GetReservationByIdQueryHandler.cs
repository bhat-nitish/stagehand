using Stagehand.Reservations.Application.Abstractions.Persistence;
using Stagehand.SharedKernel;
using Stagehand.SharedKernel.Application.Messaging;

namespace Stagehand.Reservations.Application.Reservations.GetById;

internal sealed class GetReservationByIdQueryHandler
    : IQueryHandler<GetReservationByIdQuery, Result<ReservationResponse>>
{
    private readonly IReservationRepository _reservationRepository;

    public GetReservationByIdQueryHandler(IReservationRepository reservationRepository)
    {
        _reservationRepository = reservationRepository;
    }

    public async Task<Result<ReservationResponse>> Handle(
        GetReservationByIdQuery query,
        CancellationToken cancellationToken)
    {
        var reservation = await _reservationRepository.GetByIdAsync(query.ReservationId, cancellationToken);

        if (reservation is null)
        {
            return Result.Failure<ReservationResponse>(ReservationErrors.NotFound(query.ReservationId));
        }

        return new ReservationResponse(
            reservation.Id.Value,
            reservation.ListingId,
            reservation.Quantity,
            reservation.Status.ToString(),
            reservation.CreatedAt,
            reservation.ExpiresAt,
            reservation.RejectionReason);
    }
}
