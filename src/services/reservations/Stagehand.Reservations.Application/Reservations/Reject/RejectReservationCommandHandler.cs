using Stagehand.Reservations.Application.Abstractions.Persistence;
using Stagehand.SharedKernel;
using Stagehand.SharedKernel.Application.Messaging;
using Stagehand.SharedKernel.Application.Persistence;

namespace Stagehand.Reservations.Application.Reservations.Reject;

internal sealed class RejectReservationCommandHandler : ICommandHandler<RejectReservationCommand, Result>
{
    private readonly IReservationRepository _reservationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RejectReservationCommandHandler(
        IReservationRepository reservationRepository,
        IUnitOfWork unitOfWork)
    {
        _reservationRepository = reservationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(RejectReservationCommand command, CancellationToken cancellationToken)
    {
        var reservation = await _reservationRepository.GetByIdAsync(command.ReservationId, cancellationToken);

        if (reservation is null)
        {
            return Result.Failure(ReservationErrors.NotFound(command.ReservationId));
        }

        var result = reservation.Reject(command.Reason);

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
