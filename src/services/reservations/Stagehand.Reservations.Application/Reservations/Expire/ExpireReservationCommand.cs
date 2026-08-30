using Stagehand.Reservations.Domain.Reservations;
using Stagehand.SharedKernel;
using Stagehand.SharedKernel.Application.Messaging;

namespace Stagehand.Reservations.Application.Reservations.Expire;

public sealed record ExpireReservationCommand(ReservationId ReservationId) : ICommand<Result>;
