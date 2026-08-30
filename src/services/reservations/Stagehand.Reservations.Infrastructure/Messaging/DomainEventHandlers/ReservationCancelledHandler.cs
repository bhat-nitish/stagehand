using MassTransit;
using MediatR;
using Stagehand.Reservations.Domain.Reservations.Events;

namespace Stagehand.Reservations.Infrastructure.Messaging.DomainEventHandlers;

internal sealed class ReservationCancelledHandler : INotificationHandler<ReservationCancelled>
{
    private readonly IPublishEndpoint _publishEndpoint;

    public ReservationCancelledHandler(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    public Task Handle(ReservationCancelled notification, CancellationToken cancellationToken) =>
        _publishEndpoint.Publish(
            new Contracts.Reservations.ReservationCancelled(
                notification.ReservationId.Value,
                notification.ListingId,
                notification.Quantity),
            cancellationToken);
}
