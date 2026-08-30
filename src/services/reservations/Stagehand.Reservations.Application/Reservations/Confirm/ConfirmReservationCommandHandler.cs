using Stagehand.Reservations.Application.Abstractions.Persistence;
using Stagehand.SharedKernel;
using Stagehand.SharedKernel.Application.Messaging;
using Stagehand.SharedKernel.Application.Persistence;

namespace Stagehand.Reservations.Application.Reservations.Confirm;

internal sealed class ConfirmReservationCommandHandler : ICommandHandler<ConfirmReservationCommand, Result>
{
    private readonly IReservationRepository _reservationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ConfirmReservationCommandHandler(
        IReservationRepository reservationRepository,
        IUnitOfWork unitOfWork)
    {
        _reservationRepository = reservationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(ConfirmReservationCommand command, CancellationToken cancellationToken)
    {
        var reservation = await _reservationRepository.GetByIdAsync(command.ReservationId, cancellationToken);

        if (reservation is null)
        {
            return Result.Failure(ReservationErrors.NotFound(command.ReservationId));
        }

        var result = reservation.Confirm();

        if (result.IsFailure)
        {
            return result;
        }

        try
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (ConcurrencyException)
        {
            return Result.Failure(ReservationErrors.ConcurrencyConflict(command.ReservationId));
        }

        return Result.Success();
    }
}
