using MassTransit;

namespace Stagehand.Reservations.Infrastructure.Messaging.Sagas;

public sealed class ReservationState : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public string CurrentState { get; set; } = default!;
    public Guid ListingId { get; set; }
    public int Quantity { get; set; }
    public DateTimeOffset ExpiresAt { get; set; }
}
