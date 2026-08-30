using MassTransit;
using MediatR;
using Stagehand.Contracts.Inventory;
using Stagehand.Inventory.Application.StockItems.ReserveForListing;

namespace Stagehand.Inventory.Infrastructure.Messaging.Consumers;

public sealed class ReserveStockConsumer : IConsumer<ReserveStock>
{
    private readonly ISender _sender;

    public ReserveStockConsumer(ISender sender)
    {
        _sender = sender;
    }

    public async Task Consume(ConsumeContext<ReserveStock> context)
    {
        var message = context.Message;

        var result = await _sender.Send(
            new ReserveStockForListingCommand(message.ReservationId, message.ListingId, message.Quantity),
            context.CancellationToken);

        // Success replies via the outbox (StockReserved domain event). A rejection
        // persists nothing, so there is no transaction to attach it to and it is
        // published directly.
        if (result.IsFailure)
        {
            await context.Publish(
                new StockReservationRejected(message.ReservationId, message.ListingId, result.Error.Code),
                context.CancellationToken);
        }
    }
}
