using MassTransit;
using Stagehand.Contracts.Inventory;
using Stagehand.Contracts.Reservations;

namespace Stagehand.Reservations.Infrastructure.Messaging.Sagas;

public sealed class ReservationStateMachine : MassTransitStateMachine<ReservationState>
{
    public State AwaitingStock { get; private set; } = default!;
    public State Confirmed { get; private set; } = default!;
    public State Rejected { get; private set; } = default!;
    public State Expired { get; private set; } = default!;
    public State Cancelled { get; private set; } = default!;

    public Event<ReservationRequested> Requested { get; private set; } = default!;
    public Event<StockReserved> StockReserved { get; private set; } = default!;
    public Event<StockReservationRejected> StockRejected { get; private set; } = default!;
    public Event<ReservationHoldElapsed> HoldElapsed { get; private set; } = default!;
    public Event<ReservationCancelled> CancellationReceived { get; private set; } = default!;

    public ReservationStateMachine()
    {
        InstanceState(instance => instance.CurrentState);

        Event(() => Requested, e => e.CorrelateById(m => m.Message.ReservationId));
        Event(() => StockReserved, e => e.CorrelateById(m => m.Message.ReservationId));
        Event(() => StockRejected, e => e.CorrelateById(m => m.Message.ReservationId));
        Event(() => HoldElapsed, e => e.CorrelateById(m => m.Message.ReservationId));
        Event(() => CancellationReceived, e => e.CorrelateById(m => m.Message.ReservationId));

        Initially(
            When(Requested)
                .Then(context =>
                {
                    context.Saga.ListingId = context.Message.ListingId;
                    context.Saga.Quantity = context.Message.Quantity;
                    context.Saga.ExpiresAt = context.Message.ExpiresAt;
                })
                .Send(context => new ReserveStock(
                    context.Saga.CorrelationId,
                    context.Saga.ListingId,
                    context.Saga.Quantity))
                .TransitionTo(AwaitingStock));

        During(AwaitingStock,
            When(StockReserved)
                .Send(context => new ConfirmReservation(context.Saga.CorrelationId))
                .TransitionTo(Confirmed),
            When(StockRejected)
                .Send(context => new RejectReservation(
                    context.Saga.CorrelationId,
                    context.Message.Reason))
                .TransitionTo(Rejected),
            // Stock may or may not be held yet — release is idempotent, so publish either way.
            When(HoldElapsed)
                .Send(context => new ExpireReservation(context.Saga.CorrelationId))
                .Send(context => new ReleaseStock(
                    context.Saga.CorrelationId,
                    context.Saga.ListingId,
                    ReleaseReasons.Expired))
                .TransitionTo(Expired),
            When(CancellationReceived)
                .Send(context => new ReleaseStock(
                    context.Saga.CorrelationId,
                    context.Saga.ListingId,
                    ReleaseReasons.Cancelled))
                .TransitionTo(Cancelled));

        During(Confirmed,
            When(CancellationReceived)
                .Send(context => new ReleaseStock(
                    context.Saga.CorrelationId,
                    context.Saga.ListingId,
                    ReleaseReasons.Cancelled))
                .TransitionTo(Cancelled),
            Ignore(HoldElapsed));

        // Stock confirmed after the reservation already moved on. Nothing can consume it,
        // so it must be handed back or it leaks.
        During(Expired, Cancelled,
            When(StockReserved)
                .Send(context => new ReleaseStock(
                    context.Saga.CorrelationId,
                    context.Saga.ListingId,
                    ReleaseReasons.ConfirmationTooLate)),
            Ignore(HoldElapsed),
            Ignore(StockRejected),
            Ignore(CancellationReceived));

        During(Rejected,
            Ignore(HoldElapsed),
            Ignore(StockReserved),
            Ignore(CancellationReceived));
    }
}
