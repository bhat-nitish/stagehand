namespace Stagehand.Contracts.Reservations;

/// <summary>
/// Well-known values for <see cref="ReservationHoldReleaseRequested.Reason"/>.
/// Audit/metrics vocabulary — consumers must NOT branch on these.
/// </summary>
public static class ReleaseReasons
{
    public const string Cancelled = "Cancelled";

    public const string Expired = "Expired";

    /// <summary>
    /// Stock was reserved, but the confirmation arrived after the reservation had already
    /// left Pending (cancelled or expired in the meantime), so it could not be applied.
    /// </summary>
    public const string ConfirmationTooLate = "ConfirmationTooLate";
}
