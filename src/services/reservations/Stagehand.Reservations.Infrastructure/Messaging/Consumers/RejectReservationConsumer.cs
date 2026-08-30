using MassTransit;
using MediatR;
using Stagehand.Contracts.Reservations;
using Stagehand.Reservations.Application.Reservations.Reject;
using Stagehand.Reservations.Domain.Reservations;

namespace Stagehand.Reservations.Infrastructure.Messaging.Consumers;

public sealed class RejectReservationConsumer : IConsumer<RejectReservation>
{
    private readonly ISender _sender;

    public RejectReservationConsumer(ISender sender)
    {
        _sender = sender;
    }

    public async Task Consume(ConsumeContext<RejectReservation> context)
    {
        var result = await _sender.Send(
            new RejectReservationCommand(
                new ReservationId(context.Message.ReservationId),
                context.Message.Reason),
            context.CancellationToken);

        if (result.IsFailure)
        {
            throw new InvalidOperationException(
                $"Could not reject reservation '{context.Message.ReservationId}': {result.Error.Code}.");
        }
    }
}
