using Stagehand.Reservations.Domain.Reservations.Events;
using Stagehand.SharedKernel;

namespace Stagehand.Reservations.Domain.Reservations;

public sealed class Reservation : AggregateRoot<ReservationId>
{
    public Guid ListingId { get; private set; }
    public int Quantity { get; private set; }
    public ReservationStatus Status { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset ExpiresAt { get; private set; }
    public string? RejectionReason { get; private set; }

    // EF materialises through this constructor and binds arguments by name,
    // so parameter names must match property names.
    private Reservation(
        ReservationId id,
        Guid listingId,
        int quantity,
        DateTimeOffset createdAt,
        DateTimeOffset expiresAt)
        : base(id)
    {
        ListingId = listingId;
        Quantity = quantity;
        CreatedAt = createdAt;
        ExpiresAt = expiresAt;
        Status = ReservationStatus.Pending;
    }

    public static Reservation Place(
        Guid listingId,
        int quantity,
        DateTimeOffset createdAt,
        DateTimeOffset expiresAt)
    {
        var reservation = new Reservation(ReservationId.New(), listingId, quantity, createdAt, expiresAt);
        reservation.RaiseDomainEvent(new ReservationPlaced(reservation.Id, listingId, quantity, expiresAt));
        return reservation;
    }

    public Result Confirm()
    {
        if (Status != ReservationStatus.Pending)
        {
            return Result.Failure(ReservationErrors.NotPending(Id, Status));
        }

        Status = ReservationStatus.Confirmed;
        RaiseDomainEvent(new ReservationConfirmed(Id, ListingId, Quantity));
        return Result.Success();
    }

    public Result Reject(string reason)
    {
        if (Status != ReservationStatus.Pending)
        {
            return Result.Failure(ReservationErrors.NotPending(Id, Status));
        }

        Status = ReservationStatus.Rejected;
        RejectionReason = reason;
        RaiseDomainEvent(new ReservationRejected(Id, ListingId, reason));
        return Result.Success();
    }

    public Result Expire()
    {
        if (Status != ReservationStatus.Pending)
        {
            return Result.Failure(ReservationErrors.NotPending(Id, Status));
        }

        Status = ReservationStatus.Expired;
        RaiseDomainEvent(new ReservationExpired(Id, ListingId, Quantity));
        return Result.Success();
    }

    public Result Cancel()
    {
        if (Status is not (ReservationStatus.Pending or ReservationStatus.Confirmed))
        {
            return Result.Failure(ReservationErrors.CannotCancel(Id, Status));
        }

        Status = ReservationStatus.Cancelled;
        RaiseDomainEvent(new ReservationCancelled(Id, ListingId, Quantity));
        return Result.Success();
    }
}
