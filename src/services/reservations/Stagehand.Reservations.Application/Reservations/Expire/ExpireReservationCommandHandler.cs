using Stagehand.Reservations.Application.Abstractions.Persistence;
using Stagehand.SharedKernel;
using Stagehand.SharedKernel.Application.Messaging;
using Stagehand.SharedKernel.Application.Persistence;

namespace Stagehand.Reservations.Application.Reservations.Expire;

internal sealed class ExpireReservationCommandHandler : ICommandHandler<ExpireReservationCommand, Result>
{
    private readonly IReservationRepository _reservationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ExpireReservationCommandHandler(
        IReservationRepository reservationRepository,
        IUnitOfWork unitOfWork)
    {
        _reservationRepository = reservationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(ExpireReservationCommand command, CancellationToken cancellationToken)
    {
        var reservation = await _reservationRepository.GetByIdAsync(command.ReservationId, cancellationToken);

        if (reservation is null)
        {
            return Result.Failure(ReservationErrors.NotFound(command.ReservationId));
        }

        var result = reservation.Expire();

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
