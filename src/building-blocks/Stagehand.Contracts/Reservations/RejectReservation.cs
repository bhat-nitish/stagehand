namespace Stagehand.Contracts.Reservations;

public sealed record RejectReservation(Guid ReservationId, string Reason);
