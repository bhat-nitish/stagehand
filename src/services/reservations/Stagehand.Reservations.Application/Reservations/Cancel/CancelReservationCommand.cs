using Stagehand.Reservations.Domain.Reservations;
using Stagehand.SharedKernel;
using Stagehand.SharedKernel.Application.Messaging;

namespace Stagehand.Reservations.Application.Reservations.Cancel;

public sealed record CancelReservationCommand(ReservationId ReservationId) : ICommand<Result>;
