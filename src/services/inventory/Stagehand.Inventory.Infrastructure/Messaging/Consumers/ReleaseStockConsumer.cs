using MassTransit;
using MediatR;
using Stagehand.Contracts.Inventory;
using Stagehand.Inventory.Application.StockItems.ReleaseForListing;

namespace Stagehand.Inventory.Infrastructure.Messaging.Consumers;

public sealed class ReleaseStockConsumer : IConsumer<ReleaseStock>
{
    private readonly ISender _sender;

    public ReleaseStockConsumer(ISender sender)
    {
        _sender = sender;
    }

    public async Task Consume(ConsumeContext<ReleaseStock> context)
    {
        var message = context.Message;

        var result = await _sender.Send(
            new ReleaseStockForListingCommand(message.ReservationId, message.ListingId),
            context.CancellationToken);

        if (result.IsFailure)
        {
            throw new InvalidOperationException(
                $"Could not release stock for reservation '{message.ReservationId}': {result.Error.Code}.");
        }
    }
}
