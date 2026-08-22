using Microsoft.Extensions.Options;
using Stagehand.Reservations.Application.Abstractions.Persistence;
using Stagehand.Reservations.Domain.Reservations;
using Stagehand.SharedKernel;
using Stagehand.SharedKernel.Application.Messaging;
using Stagehand.SharedKernel.Application.Persistence;

namespace Stagehand.Reservations.Application.Reservations.Place;

internal sealed class PlaceReservationCommandHandler
    : ICommandHandler<PlaceReservationCommand, Result<ReservationId>>
{
    private readonly IReservationRepository _reservationRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly TimeProvider _timeProvider;
    private readonly ReservationOptions _options;

    public PlaceReservationCommandHandler(
        IReservationRepository reservationRepository,
        IUnitOfWork unitOfWork,
        TimeProvider timeProvider,
        IOptions<ReservationOptions> options)
    {
        _reservationRepository = reservationRepository;
        _unitOfWork = unitOfWork;
        _timeProvider = timeProvider;
        _options = options.Value;
    }

    public async Task<Result<ReservationId>> Handle(
        PlaceReservationCommand command,
        CancellationToken cancellationToken)
    {
        var now = _timeProvider.GetUtcNow();
        var expiresAt = now.AddMinutes(_options.HoldMinutes);

        var reservation = Reservation.Place(command.ListingId, command.Quantity, now, expiresAt);

        _reservationRepository.Add(reservation);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return reservation.Id;
    }
}
