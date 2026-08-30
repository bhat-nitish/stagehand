using Stagehand.Reservations.Domain.Reservations;
using Stagehand.SharedKernel;
using Stagehand.SharedKernel.Application.Messaging;

namespace Stagehand.Reservations.Application.Reservations.Reject;

public sealed record RejectReservationCommand(ReservationId ReservationId, string Reason) : ICommand<Result>;
