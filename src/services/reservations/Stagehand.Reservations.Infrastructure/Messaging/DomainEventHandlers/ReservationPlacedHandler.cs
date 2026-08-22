using MassTransit;
using MediatR;
using Stagehand.Contracts.Reservations;
using Stagehand.Reservations.Domain.Reservations.Events;

namespace Stagehand.Reservations.Infrastructure.Messaging.DomainEventHandlers;

internal sealed class ReservationPlacedHandler : INotificationHandler<ReservationPlaced>
{
    private readonly IPublishEndpoint _publishEndpoint;

    public ReservationPlacedHandler(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    public Task Handle(ReservationPlaced notification, CancellationToken cancellationToken) =>
        _publishEndpoint.Publish(
            new ReservationRequested(
                notification.ReservationId.Value,
                notification.ListingId,
                notification.Quantity,
                notification.ExpiresAt),
            cancellationToken);
}
