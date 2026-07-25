using Stagehand.Reservations.Domain.Reservations;
using Stagehand.SharedKernel;
using Stagehand.SharedKernel.Application.Messaging;

namespace Stagehand.Reservations.Application.Reservations.Place;

public sealed record PlaceReservationCommand(Guid ListingId, int Quantity)
    : ICommand<Result<ReservationId>>;
