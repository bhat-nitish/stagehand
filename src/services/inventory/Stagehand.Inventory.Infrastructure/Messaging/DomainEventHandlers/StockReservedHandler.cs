using MassTransit;
using MediatR;
using Stagehand.Inventory.Domain.StockItems.Events;

namespace Stagehand.Inventory.Infrastructure.Messaging.DomainEventHandlers;

internal sealed class StockReservedHandler : INotificationHandler<StockReserved>
{
    private readonly IPublishEndpoint _publishEndpoint;

    public StockReservedHandler(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    public Task Handle(StockReserved notification, CancellationToken cancellationToken) =>
        _publishEndpoint.Publish(
            new Contracts.Inventory.StockReserved(
                notification.ReservationId,
                notification.ListingId,
                notification.Quantity),
            cancellationToken);
}
