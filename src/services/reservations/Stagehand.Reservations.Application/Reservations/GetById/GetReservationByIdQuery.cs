using Stagehand.Reservations.Domain.Reservations;
using Stagehand.SharedKernel;
using Stagehand.SharedKernel.Application.Messaging;

namespace Stagehand.Reservations.Application.Reservations.GetById;

public sealed record GetReservationByIdQuery(ReservationId ReservationId)
    : IQuery<Result<ReservationResponse>>;
