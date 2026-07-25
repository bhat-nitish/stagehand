namespace Stagehand.Reservations.Application;

public sealed class ReservationOptions
{
    public const string SectionName = "Reservations";

    /// <summary>
    /// How long a placed reservation holds its claim before expiring. Kept short in
    /// tests so the expiry path can be exercised without waiting.
    /// </summary>
    public int HoldMinutes { get; set; } = 10;
}
