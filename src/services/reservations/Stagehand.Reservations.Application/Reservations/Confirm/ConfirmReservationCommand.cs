using Stagehand.Reservations.Domain.Reservations;
using Stagehand.SharedKernel;
using Stagehand.SharedKernel.Application.Messaging;

namespace Stagehand.Reservations.Application.Reservations.Confirm;

public sealed record ConfirmReservationCommand(ReservationId ReservationId) : ICommand<Result>;
