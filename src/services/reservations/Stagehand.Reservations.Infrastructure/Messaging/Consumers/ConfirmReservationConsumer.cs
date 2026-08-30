using MassTransit;
using MediatR;
using Stagehand.Contracts.Reservations;
using Stagehand.Reservations.Application.Reservations.Confirm;
using Stagehand.Reservations.Domain.Reservations;

namespace Stagehand.Reservations.Infrastructure.Messaging.Consumers;

public sealed class ConfirmReservationConsumer : IConsumer<ConfirmReservation>
{
    private const string NotPending = "Reservation.NotPending";

    private readonly ISender _sender;

    public ConfirmReservationConsumer(ISender sender)
    {
        _sender = sender;
    }

    public async Task Consume(ConsumeContext<ConfirmReservation> context)
    {
        var result = await _sender.Send(
            new ConfirmReservationCommand(new ReservationId(context.Message.ReservationId)),
            context.CancellationToken);

        // NotPending means the reservation was cancelled or expired first. The saga's
        // cancel/expire path releases the stock, so this is a race rather than a fault.
        if (result.IsFailure && result.Error.Code != NotPending)
        {
            throw new InvalidOperationException(
                $"Could not confirm reservation '{context.Message.ReservationId}': {result.Error.Code}.");
        }
    }
}
