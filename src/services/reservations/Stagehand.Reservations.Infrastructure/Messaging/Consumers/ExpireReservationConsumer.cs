using MassTransit;
using MediatR;
using Stagehand.Contracts.Reservations;
using Stagehand.Reservations.Application.Reservations.Expire;
using Stagehand.Reservations.Domain.Reservations;

namespace Stagehand.Reservations.Infrastructure.Messaging.Consumers;

public sealed class ExpireReservationConsumer : IConsumer<ExpireReservation>
{
    private const string NotPending = "Reservation.NotPending";

    private readonly ISender _sender;

    public ExpireReservationConsumer(ISender sender)
    {
        _sender = sender;
    }

    public async Task Consume(ConsumeContext<ExpireReservation> context)
    {
        var result = await _sender.Send(
            new ExpireReservationCommand(new ReservationId(context.Message.ReservationId)),
            context.CancellationToken);

        // NotPending means the reservation was cancelled or confirmed in the meantime.
        // That is a legitimate race, not a fault.
        if (result.IsFailure && result.Error.Code != NotPending)
        {
            throw new InvalidOperationException(
                $"Could not expire reservation '{context.Message.ReservationId}': {result.Error.Code}.");
        }
    }
}
